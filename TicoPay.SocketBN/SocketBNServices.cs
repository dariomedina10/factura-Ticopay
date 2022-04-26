using Abp.Dependency;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using TicoPay.Users;

namespace TicoPay
{
    public class SocketBNServices : ITransientDependency
    {
        private readonly IRepository<Tenant, int> _userRepository;

        public SocketBNServices(IRepository<Tenant, int> userRepository)
        {
            _userRepository = userRepository;

        }

        public void Run()
        {
            //GetAllList
            foreach (var user in _userRepository.GetAllList())
            {
                Console.WriteLine(user);
            }

            //Get
            Console.WriteLine("Halil: " + _userRepository.Get(1));

        }
    }
}
