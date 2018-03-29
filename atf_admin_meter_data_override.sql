USE [atf];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO


ALTER PROCEDURE [dbo].[atf_admin_meter_data_override]
	@override_list [dbo].[schedule_key] READONLY, 
	@user_name varchar(256)
AS
BEGIN 		 
	DECLARE
		@sm_id					TINYINT, 
		@ct_dt					SMALLDATETIME, 
		@process_id			INT,
		@return					SMALLINT = 0,
		@error					SMALLINT = 0, 
		@atf_date_1			DATETIME,  
		@atf_date_2			DATETIME;   
	
		BEGIN TRANSACTION
		
			BEGIN TRY
				SET NOCOUNT ON
				SET NOCOUNT ON --Test for git 

				SELECT  @process_id = process_ID 
				FROM app_processes
				WHERE process_code = 'METER_OVERRIDE'; 

				EXECUTE @error = meter_override_sm_rollup_items @override_list, @user_name; 
				
				IF @error = 0 
					EXECUTE @error = meter_override_must_run_units @override_list, @user_name; 
					
				IF @error = 0 
					EXECUTE @error = meter_override_test_energy @override_list, @user_name; 

				--Update decatherm values via  atf_valdtr_gas_dt_calcs
				IF @error = 0 
					BEGIN 
						DECLARE override_sm_date_cur CURSOR FORWARD_ONLY 
						FOR 
							SELECT sm_id, ct_dt
							FROM @override_list; 

						OPEN override_sm_date_cur; 
					
						FETCH NEXT FROM override_sm_date_cur INTO @sm_id, @ct_dt; 

						WHILE (SELECT fetch_status FROM sys.dm_exec_cursors(@@SPID) WHERE name = 'override_sm_date_cur') = 0 
							BEGIN 
								 --CT Date covers two ATF dates - we need to process dekatherms for both days
								SET @atf_date_1 = DATEADD(HH, 9, DATEADD(DD, -1, @ct_dt)); 
								SET @atf_date_2 = DATEADD(HH, 9, @ct_dt);
                
								--Recalculate dekatherms for the first ATF date
								EXECUTE @error = atf_valdtr_gas_dt_calcs @sm_id, @atf_date_1;
								
								IF @error = 0
									--Recalculate dekatherms for the second ATF date
								  EXECUTE @error = atf_valdtr_gas_dt_calcs @sm_id, @atf_date_2;
								
								IF @error = 0 
									FETCH NEXT FROM override_sm_date_cur INTO @sm_id, @ct_dt;
								ELSE 
									BREAK; 
								END; 
					
						CLOSE override_sm_date_cur; 
						DEALLOCATE override_sm_date_cur; 
					END; 
				
				--Log process completion 
				IF @error = 0 
					INSERT INTO app_completed_processes
						(	sm_id, 
							process_ID, 
							date_processed, 
							processed_date_type, 
							user_name, 
							completion_date )
					SELECT 
						sm_id, 
						@process_id, 
						ct_dt,
						'CENTRAL', 
						@user_name, 
						GETDATE()			
					FROM @override_list; 
			END TRY 

			BEGIN CATCH 
				SET @return = -1;
				SET @error = -1; 

				DECLARE @ErrMsg	VARCHAR(2000);
				SET @ErrMsg = dbo.log_FormatSQLMsg(Error_Message(), Error_Procedure(), Error_Line(), Error_Severity());
				EXEC log_AddEntry 'Warning', @ErrMsg;

				IF CURSOR_STATUS('global','override_sm_date_cur')  IN (0, 1) 
					BEGIN 
						CLOSE override_sm_date_cur; 
						DEALLOCATE override_sm_date_cur; 
					END ; 
			END CATCH 
	
	--The three override processes either fail or succeed as a whole - if any one fails, rollback all commits 
	IF @error = 0 
		COMMIT TRANSACTION;
	ELSE 
		ROLLBACK TRANSACTION;

	RETURN @return;
END

GO
