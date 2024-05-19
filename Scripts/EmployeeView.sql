USE Library_fund
GO

DROP VIEW EmployeeView
GO

CREATE VIEW EmployeeView AS
SELECT employee.first_name + N' ' + employee.last_name as full_name, employee.phone_number, position.name as position, library.name as library
FROM employee 
INNER JOIN position ON employee.position_id = position.id
INNER JOIN library ON employee.library_id = library.id
GO

SELECT *
FROM EmployeeView