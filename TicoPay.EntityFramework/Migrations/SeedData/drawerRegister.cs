using System.Linq;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class drawerRegisterSeed
    {
        private readonly TicoPayDbContext _context;

        public drawerRegisterSeed(TicoPayDbContext context)
        {
            _context = context;
        }

        public void create()
        {
            string query = string.Empty;
            var result = 0;

            query = @"insert into [TicoPay].[BranchOffices] ([Id], [Name], [Code], [Location], [IsDeleted], [TenantId],  [CreationTime])
                            select NEWID(), 'Principal','001', '',0,Id, GETDATE() from [TicoPay].[AbpTenants] t where not exists (
                    select id from [TicoPay].[BranchOffices] b where b.TenantId=t.Id and b.Code='001')";
            result = _context.Database.ExecuteSqlCommand(query);

            query = @"insert into [TicoPay].[Drawers] ([Id], [Code], [Description], [IsDeleted], [TenantId], [BranchOfficeId], [IsOpen], [CreationTime])
                    select NEWID(),'00001','Caja 01', 0, b.TenantId,b.Id,0,GETDATE() from [TicoPay].[BranchOffices] b where Code='001' 
                    and IsDeleted=0 and not exists (select * from [TicoPay].[Drawers] d where b.Id=d.BranchOfficeId and d.Code='00001')";
            result = _context.Database.ExecuteSqlCommand(query);

            //and r.Name = 'Admin'

            query = @"insert into[TicoPay].[DrawerUsers] ([Id], [IsDeleted], [TenantId], [DrawerId], [IsActive], [CreationTime],[User_Id])
                select NEWID(),0,u.TenantId,d.Id,1,GETDATE(),u.Id
                                    from [TicoPay].[AbpUsers] u
					                inner join [TicoPay].[AbpUserRoles] ur on u.Id=ur.UserId  
					                inner join[TicoPay].[AbpRoles] r on ur.RoleId = r.Id  and  r.TenantId=u.TenantId
					                inner join [TicoPay].[Drawers] d on u.TenantId=d.TenantId and d.Code='00001'
					                where u.IsActive = 1 and not exists (				
                select id from [TicoPay].[DrawerUsers] du where du.[User_Id]=u.Id and DrawerId=d.Id) and u.creationtime < DATEFROMPARTS ( 2018, 08, 25 ) ";
            result = _context.Database.ExecuteSqlCommand(query);

            query = @"update [TicoPay].[Invoices] set DrawerId=d.Id  from [TicoPay].[Invoices] i
                inner join [TicoPay].[Drawers] d on i.TenantId=d.TenantId and d.Code='00001' and d.IsDeleted=0 and i.DrawerId is null";
            result = _context.Database.ExecuteSqlCommand(query);

            query = @"update [TicoPay].[Vouchers] set DrawerId=d.Id  from [TicoPay].[Vouchers] i
                    inner join [TicoPay].[Drawers] d on i.TenantId=d.TenantId and d.Code='00001' and d.IsDeleted=0 and i.DrawerId is null";
            result = _context.Database.ExecuteSqlCommand(query);


            query = @"update [TicoPay].[Notes] set DrawerId=d.Id  from [TicoPay].[Notes] i
                    inner join [TicoPay].[Drawers] d on i.TenantId=d.TenantId and d.Code='00001' and d.IsDeleted=0 and i.DrawerId is null";
            result = _context.Database.ExecuteSqlCommand(query);

            query = @"update [TicoPay].[Registers] set DrawerId=d.Id  from [TicoPay].[Registers] i
                inner join [TicoPay].[Drawers] d on i.TenantId=d.TenantId and i.RegisterCode=d.Code  and d.IsDeleted=0 and i.DrawerId is null";
            result = _context.Database.ExecuteSqlCommand(query);

            //query = @"Update TicoPay.Drawers set IsOpen=1, UserIdOpen=du.User_Id from TicoPay.Drawers d 
            //            inner join [TicoPay].[DrawerUsers] du on d.id=du.DrawerId and Code='00001' where  exists (select DrawerId
            //            from  [TicoPay].[DrawerUsers] dd  where dd.DrawerId=d.Id group by DrawerId having count(User_Id)=1)";
            //result = _context.Database.ExecuteSqlCommand(query);


        }

        public void FixOpenDefaultDrawer()
        {
            var tenants = _context.Tenants.Where(r => !r.IsDeleted).Select(h=>h.Id).ToList();
            foreach (var Id in tenants)
            {
                var drawerDefault = _context.Drawers.Where(d => !d.IsOpen && d.TenantId == Id && d.Code == "00001").FirstOrDefault();
                if (drawerDefault != null)
                {
                    var user = _context.Users.Where(u => u.TenantId == Id).FirstOrDefault();
                    if (user != null)
                    {
                        drawerDefault.IsOpen = true;
                        drawerDefault.UserIdOpen = user.Id;

                    }
                    _context.SaveChanges();
                }
            }
        }
    }
}
