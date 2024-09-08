CREATE TABLE Tasks
(
    task_no INT IDENTITY(1,1) PRIMARY KEY, -- Auto-incrementing primary key
    username NVARCHAR(50) NOT NULL,        -- User's name (adjust length as needed)
    task_name NVARCHAR(100) NOT NULL,      -- Task name (adjust length as needed)
    task_date DATE NOT NULL,               -- Date of the task
    task_details NVARCHAR(MAX) NULL        -- Details about the task (optional)
);

select * from tasks

CREATE PROCEDURE DeleteTasksBeforeToday
AS
BEGIN	
    -- Fetch the current date
    DECLARE @CurrentDate DATE = CAST(GETDATE() AS DATE);

    -- Delete rows with task_date before today
    DELETE FROM Tasks
    WHERE task_date < @CurrentDate;
END;

EXEC DeleteTasksBeforeToday
