  SELECT p.Id, p.Name, p.Price, p.Description,p.IsFeatured, p.IsDeleted, COUNT(op.ProductsId) AS SoldCount
                            FROM dbo.Products p
                            INNER JOIN OrderProduct op ON p.Id = op.ProductsId
                            GROUP BY p.Id, p.Name, p.Price, p.Description, p.IsFeatured, p.IsDeleted
                            ORDER BY SoldCount DESC