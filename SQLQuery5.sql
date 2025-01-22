SELECT p.Id, p.Name, p.Price, p.Description,p.IsFeatured, p.IsDeleted,c.Name, COUNT(op.ProductsId) AS SoldCount
                            FROM dbo.Products p
                            INNER JOIN OrderProduct op ON p.Id = op.ProductsId
                            INNER JOIN CategoryProduct cp ON cp.ProductsId = p.Id
                            INNER JOIN Categories c ON c.Id = cp.CategoriesId
                            GROUP BY p.Id, p.Name, p.Price, p.Description, p.IsFeatured, p.IsDeleted, c.Name
                            ORDER BY SoldCount DESC