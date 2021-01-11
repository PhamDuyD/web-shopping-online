using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingOnline.Models.Business
{
    public class UserBusiness
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();

        public User findUser(User user)
        {
            return db.Users.Single(x => x.Account == user.Account && x.Password == user.Password);
        }

        public User findID(long ID)
        {
            return db.Users.Find(ID);
        }

        public bool checkLogin(User user)
        {
            if(db.Users.Count(x => x.Account == user.Account && x.Password == user.Password) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool register(User entity)
        {
            try
            {
                var user = new User();
                user.Account = entity.Account.Trim();
                user.Password = entity.Password.Trim();
                user.Fullname = entity.Fullname;
                user.Address = entity.Address;
                user.Phone = entity.Phone;
                user.Status = true;

                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void changePass(long ID, string Password)
        {
            var user = db.Users.Find(ID);
            user.Password = Password.Trim();
            db.SaveChanges();
        }

        //check mk trùng hay k
        public bool checkPass(string ex_Password, long ID)
        {
            var user = db.Users.Find(ID);
            if (user.Password.Trim() == ex_Password)
                return true;
            else
                return false;
        }
    }
}