using System;
using System.Reflection;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyIoc.AutoFac
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void can_resolver_myClass()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyClass>();

            IContainer container = builder.Build();
            var myClass = container.Resolve<MyClass>();

        }
        [TestMethod]
        public void register_as_interface()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new MyClass()).As<MyInetface>();
            builder.RegisterType<MyClass>();

            IContainer container = builder.Build();
            var myClass = container.Resolve<MyInetface>();

        }

        [TestMethod]
        public void register_with_parameter()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new MyParameter());
            builder.Register(c => new MyClass(c.Resolve<MyParameter>()));

            var container = builder.Build();
            var result = container.Resolve<MyClass>();
        }

        [TestMethod]
        public void register_with_property()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new MyProperty());
            builder.Register(c => new MyClass()
            {
                p = c.Resolve<MyProperty>()
            });

            var container = builder.Build();
            var myClass = container.Resolve<MyClass>();

        }
        [TestMethod]
        public void select_an_implementer_base_on_parameter_value()
        {
            var builder = new ContainerBuilder();
            builder.Register<MyInetface>((c, p) =>
            {
                var type = p.Named<string>("type");
                if (type == "test")
                {
                    return new MyClass();
                }
                else
                    return new MyClass();
            });
        }

        [TestMethod]
        public void register_with_instance()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(MyInstance.Instance).ExternallyOwned();
            IContainer container = builder.Build();
            var myInstance1 = container.Resolve<MyInstance>();
            var myInstance2 = container.Resolve<MyInstance>();
        }
        [TestMethod]
        public void register_order_defaults()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<MyClass>().As<MyInetface>();
            containerBuilder.RegisterType<MyClass>().As<MyInetface>().PreserveExistingDefaults();

            IContainer container = containerBuilder.Build();
            var repository = container.Resolve<MyInetface>();
        }
        [TestMethod]
        public void register_with_name()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<MyClass>().Named<MyInetface>("DB");
            containerBuilder.RegisterType<MyClass>().Named<MyInetface>("Test");

            IContainer container = containerBuilder.Build();
            var dbRepository = container.ResolveNamed<MyInetface>("DB");
            var testRepository = container.ResolveNamed<MyInetface>("Test");
        }

        [TestMethod]
        public void register_assembly()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).
                Where(t => t.Name.EndsWith("Repository")).
                AsImplementedInterfaces();

            IContainer container = builder.Build();
            var repository = container.Resolve<MyInetface>();
        }



    }
    public class MyInstance
    {
        private MyInstance() { }
        public static MyInstance Instance { get; private set; }
        static MyInstance()
        {
            Instance = new MyInstance();
        }
    }
    public interface MyInetface
    { }
    public class MyProperty
    { }
    public class MyClass : MyInetface
    {
        public MyClass() { }
        public MyClass(MyParameter p)
        {
            MyParameter = p;
        }
        public MyProperty p { get; set; }
        public MyParameter MyParameter { get; private set; }
        public string Name { get; set; }
    }
    public class MyParameter
    { }
}
