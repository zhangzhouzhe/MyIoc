using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyIoc.Cat
{
    public class Class1
    {
    }

    public interface IFoo { }
    public interface IBar { }
    public interface IBaz { }
    public interface IQux { }
    public interface IFoobar<T1, T2> { }


    public class Base : IDisposable
    {
        public Base()
            => Console.WriteLine($"{GetType().Name} is Created");
        public void Dispose()
            => Console.WriteLine($"{GetType().Name} is disposed");
    }
    public class Foo : Base, IFoo { }

    public class Bar : Base, IBar { }

    public class Baz : Base, IBaz { }

    public class Qux : Base, IQux { }

    public class Foobar<T1, T2> : Base, IFoobar<T1, T2> { }

    public enum Lifetime
    {
        Root,
        Self,
        Transient
    }
    public class ServiceRegistry
    {
        public Type ServiceType { get; }
        public Lifetime Lifetime { get; }
        public Func<Cat, Type[], object> Factory { get; }
        internal ServiceRegistry Next { get; set; }

        public ServiceRegistry(Type serviceType, Lifetime lifetime,
            Func<Cat, Type[], object> factory)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
            Factory = factory;
        }
        internal IEnumerable<ServiceRegistry> AsEnumerable()
        {
            var list = new List<ServiceRegistry>();
            for (var self = this; self != null; self = self.Next)
            {
                list.Add(self);
            }
            return list;
        }
    }
    public class Cat : IServiceProvider, IDisposable
    {
        internal readonly Cat _root;
        internal readonly ConcurrentDictionary<Type, ServiceRegistry> _registries;
        internal readonly ConcurrentDictionary<Key, object> _services;
        private readonly ConcurrentBag<IDisposable> _disposables;
        private volatile bool _dispoed;

        public Cat()
        {
            _root = this;
            _registries = new ConcurrentDictionary<Type, ServiceRegistry>();
            _services = new ConcurrentDictionary<Key, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }
        internal Cat(Cat parent)
        {
            _root = parent._root;
            _registries = new ConcurrentDictionary<Type, ServiceRegistry>();
            _services = new ConcurrentDictionary<Key, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }
        private void EnsuerNotDispoed()
        {
            if (_dispoed)
                throw new ObjectDisposedException("Cat");
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }

    internal class Key : IEquatable<Key>
    {
        public ServiceRegistry Registry { get; }
        public Type[] GenericArguments { get; }
        public Key(ServiceRegistry serviceRegistry, Type[] genericArguments)
        {
            Registry = serviceRegistry;
            GenericArguments = genericArguments;
        }

        public bool Equals(Key other)
        {
            if (Registry != other.Registry)
                return false;

            if (GenericArguments.Length != other.GenericArguments.Length)
                return false;

            for (int i = 0; i < GenericArguments.Length; i++)
            {
                if (GenericArguments[i] != other.GenericArguments[i])
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = Registry.GetHashCode();
            for (int i = 0; i < GenericArguments.Length; i++)
            {
                hashCode ^= GenericArguments[i].GetHashCode();
            }
            return hashCode;
        }
    }

}



