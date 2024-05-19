USE Library_fund
GO

DROP VIEW BookCopyInfo
GO

CREATE VIEW BookCopyInfo AS
SELECT book.name AS book_name, author.name as author, book_copy.publisher, book_copy.year
FROM book_copy 
INNER JOIN book ON book.id = book_copy.book_id
INNER JOIN book_author on book.id = book_author.book_id
INNER JOIN author on book_author.author_id = author.id
GO

SELECT *
FROM BookCopyInfo