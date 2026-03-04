-- Пример 1: Извлечение простых значений
SELECT 
    BookID,
    Title,
    CAST(SchemaXml AS XML).value('(/BookSchema/Title)[1]', 'NVARCHAR(200)') AS XmlTitle,
    CAST(SchemaXml AS XML).value('(/BookSchema/Author)[1]', 'NVARCHAR(100)') AS XmlAuthor,
    CAST(SchemaXml AS XML).value('count(/BookSchema/Parts/Part)', 'INT') AS PartCount
FROM Books
WHERE SchemaXml IS NOT NULL

-- Выборка всех глав (CROSS APPLY + nodes)
SELECT 
    b.BookID,
    b.Title,
    T.c.value('@number', 'INT') AS ChapterNumber,
    T.c.value('.', 'NVARCHAR(200)') AS ChapterTitle
FROM Books b
CROSS APPLY (SELECT CAST(b.SchemaXml AS XML)) AS X(XmlData)
CROSS APPLY X.XmlData.nodes('/BookSchema/Parts/Part/Chapter') AS T(c)
WHERE b.SchemaXml IS NOT NULL
ORDER BY b.BookID, ChapterNumber

-- Пример 3: Выборка с группировкой по частям
SELECT 
    b.BookID,
    b.Title,
    P.c.value('@number', 'INT') AS PartNumber,
    C.c.value('@number', 'INT') AS ChapterNumber,
    C.c.value('.', 'NVARCHAR(200)') AS ChapterTitle
FROM Books b
CROSS APPLY (SELECT CAST(b.SchemaXml AS XML)) AS X(XmlData)
CROSS APPLY X.XmlData.nodes('/BookSchema/Parts/Part') AS P(c)
CROSS APPLY P.c.nodes('Chapter') AS C(c)
WHERE b.SchemaXml IS NOT NULL
ORDER BY b.BookID, PartNumber, ChapterNumber

-- Найти книги, где в названии главы есть "Раскольников"
SELECT 
    b.BookID,
    b.Title,
    C.c.value('.', 'NVARCHAR(200)') AS ChapterTitle
FROM Books b
CROSS APPLY (SELECT CAST(b.SchemaXml AS XML)) AS X(XmlData)
CROSS APPLY X.XmlData.nodes('/BookSchema/Parts/Part/Chapter') AS C(c)
WHERE b.SchemaXml IS NOT NULL
  AND C.c.value('.', 'NVARCHAR(200)') LIKE '%Раскольников%'

-- Получить главы из Части 1
SELECT 
    b.BookID,
    b.Title,
    C.c.value('@number', 'INT') AS ChapterNumber,
    C.c.value('.', 'NVARCHAR(200)') AS ChapterTitle
FROM Books b
CROSS APPLY (SELECT CAST(b.SchemaXml AS XML)) AS X(XmlData)
CROSS APPLY X.XmlData.nodes('/BookSchema/Parts/Part[@number="1"]/Chapter') AS C(c)
WHERE b.BookID = 1
  AND b.SchemaXml IS NOT NULL