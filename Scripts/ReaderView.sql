USE Library_fund
GO

DROP VIEW ReaderView
GO

CREATE VIEW ReaderView AS
SELECT reader.first_name, reader.last_name, reader.phone_number, reader_category.name AS category, library_fund.city_name AS city
FROM reader 
INNER JOIN reader_category ON reader.category_id = reader_category.id
INNER JOIN library_fund ON reader.library_fund_id = library_fund.id
GO

SELECT *
FROM ReaderView