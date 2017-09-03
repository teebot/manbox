using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace ManBox.Common.Injection
{
    public class DependencyContainer
    {
        static DependencyContainer _instance;
        Container _injectorContainer;

        public static DependencyContainer GetInstance() {
            if (_instance == null) {
                _instance = new DependencyContainer();
            }

            return _instance;
        }

        private DependencyContainer()
        {
            _injectorContainer = new Container();
        }

        public T Resolve<T>() where T : class
        {
            return _injectorContainer.GetInstance<T>();
        }

        public void Register<TService, TImplementation>() 
            where TService : class 
            where TImplementation : class, TService
        {
            _injectorContainer.Register<TService, TImplementation>();
        }

        public void Verify() {
            _injectorContainer.Verify();
        }
    }
}
