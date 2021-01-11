using ShoppingOnline.Models.DTO;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingOnline.Models.Business
{
    public class ProductBusiness
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();

        public Product findID(long ID)
        {
            return db.Products.Find(ID);
        }

        //Tìm kiếm sản phẩm theo tên sp
        public Product searchProduct(string product_name)
        {
            return db.Products.Single(x => x.Product_Name == product_name);
        }

        //Cộng tồn kho
        public void AddQuantity(long ID, int quantity)
        {
            var product = db.Products.Find(ID);
            product.Quantity += quantity;
            db.SaveChanges();
        }

        //Trừ tồn kho sau khi thanh toán đơn hàng
        public void Subtract_Quantity(long ID, int quantity)
        {
            var product = db.Products.Find(ID);
            product.Quantity -= quantity;
            db.SaveChanges();
        }

        public List<Product> getAll()
        {
            var lst_id = db.Products.Max(x => x.ID);
            
            var lst_pro = new List<Product>();
            int dem = 0;
            var random = new Random();
            while (true)
            {
                if (dem == 6)
                    break;
                long index = random.Next((int)lst_id);
                var product = db.Products.Find(index);
                if(product != null)
                {
                    lst_pro.Add(product);
                    dem++;
                }
                   
            }

            return lst_pro;   
        }

        //lấy sản phẩm theo danh mục phụ kiện thời trang
        public List<Product> getProductBy_PhuKienThoiTrang()
        {
            return db.Products.Where(x => x.Category_ID == 1).Take(4).ToList();
        }

        //Danh mục Quần áo nam
        public List<Product> GetProductBy_QuanAoNam()
        {
            return db.Products.Where(x => x.Category_ID == 2).Take(4).ToList();
        }

        //Danh mục Quần áo nữ
        public List<Product> GetProductBy_QuanAoNu()
        {
            return db.Products.Where(x => x.Category_ID == 3).Take(4).ToList();
        }

        //Danh mục balo vali túi xách
        public List<Product> GetProductBy_ValiBalo()
        {
            return db.Products.Where(x => x.Category_ID == 4).Take(4).ToList();
        }

        //Lấy sản phẩm gợi ý
        public List<Product> getProductRecommend()
        {
            var lst_id = db.Products.Max(x => x.ID);

            var lst_pro = new List<Product>();
            int dem = 0;
            var random = new Random();
            while (true)
            {
                if (dem == 3)
                    break;
                long index = random.Next((int)lst_id);
                var product = db.Products.Find(index);
                if (product != null)
                {
                    lst_pro.Add(product);
                    dem++;
                }

            }

            return lst_pro;
        }

        //Chi tiết sản phẩm
        public ProductDTO getProductDetail(long ID)
        {
            var query = from pro in db.Products
                        join cet in db.Categories on pro.Category_ID equals cet.ID
                        join pvid in db.Providers on pro.Provider_ID equals pvid.ID
                        select new ProductDTO()
                        {
                            ID = pro.ID,
                            Product_Name = pro.Product_Name,
                            Product_Code = pro.Product_Code,
                            Metatitle = pro.Metatitle.Trim(),
                            Object = pro.Object,
                            Promotion_Price = pro.Promotion_Price,
                            Price = pro.Price,
                            Image = pro.Image,
                            Desription = pro.Desription,
                            Category_Name = cet.Name,
                            Provider_Name = pvid.Name,
                            Category_ID = pro.Category_ID
                        };
            return query.Single(x => x.ID == ID);
        }

        public List<Serial_Type> GetSerial_TypesByID(long product_ID)
        {
            return db.Serial_Type.Where(x => x.Product_ID == product_ID).ToList();
        }

        //Lấy sản phẩm cùng loại
        public List<Product> getProductByCategoryID(long? category_id)
        {
            return db.Products.Where(x => x.Category_ID == category_id).ToList();
        }

        //thêm review sản phẩm
        public bool addReview(Review entity)
        {
            try
            {
                db.Reviews.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //lấy review sản phẩm
        public List<ReviewDTO> getReview(long product_id)
        {
            var query = from rev in db.Reviews
                        join pro in db.Products on rev.Product_ID equals pro.ID
                        join user in db.Users on rev.User_ID equals user.ID
                        where rev.Product_ID == product_id
                        select new ReviewDTO()
                        {
                            ID = rev.ID,
                            Content = rev.Content,
                            Rating = rev.Rating,
                            Fullname = user.Fullname,
                            CreatedDate = rev.CreatedDate
                        };
            return query.OrderByDescending(x => x.CreatedDate).ToList();
        }

        
    }
}