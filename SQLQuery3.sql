SELECT p.Id, p.Name, p.Price, p.Description,p.IsFeatured, p.IsDeleted, u.City, u.Country,COUNT(op.ProductsId) AS SoldCount
                            FROM dbo.Products p
                            INNER JOIN OrderProduct op ON p.Id = op.ProductsId
                            INNER JOIN Orders o ON op.OrdersId = o.Id
                            INNER JOIN Users u ON o.CustomerId = u.Id
                            GROUP BY u.Country, u.City,p.Id, p.Name, p.Price, p.Description, p.IsFeatured, p.IsDeleted
                            ORDER BY SoldCount DESC