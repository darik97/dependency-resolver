using IocExample.Classes;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IocExample
{
    class Program
    {
        static void Main_Example(string[] args)
        {
            var logger = new ConsoleLogger();
            var sqlConnectionFactory = new SqlConnectionFactory("SQL Connection", logger);
            var createUserHandler = new CreateUserHandler(new UserService(new QueryExecutor(sqlConnectionFactory),
            new CommandExecutor(sqlConnectionFactory), new CacheService(logger, new RestClient("API KEY"))), logger);

            createUserHandler.Handle();
        }

        static void Main_1(string[] args)
        {
            IKernel kernel = new StandardKernel();

            kernel.Bind<CreateUserHandler>().To<CreateUserHandler>();
            kernel.Bind<UserService>().To<UserService>();
            kernel.Bind<ILogger>().To<ConsoleLogger>();
            kernel.Bind<QueryExecutor>().To<QueryExecutor>();
            kernel.Bind<CommandExecutor>().To<CommandExecutor>();
            kernel.Bind<CacheService>().
             ToConstructor(k => new CacheService(k.Inject<ILogger>(), new RestClient("API KEY")));
            kernel.Bind<IConnectionFactory>()
             .ToConstructor(k => new SqlConnectionFactory("SQL Connection", k.Inject<ILogger>()))
             .InSingletonScope();

            var createUserHandler = kernel.Get<CreateUserHandler>();
            createUserHandler.Handle();
        }


        static void Main_2(string[] args)
        { 
            var kernel = new MyKernel();

            kernel.Register<CreateUserHandler, CreateUserHandler>();
            kernel.Register<UserService, UserService>();
            kernel.Register<ILogger, ConsoleLogger>();
            kernel.Register<QueryExecutor, QueryExecutor>();
            kernel.Register<CommandExecutor, CommandExecutor>();

            kernel.Register<CacheService, CacheService>(new CacheService(kernel.Get<ILogger>(), new RestClient("API KEY")));
            kernel.Register<IConnectionFactory, SqlConnectionFactory>(new SqlConnectionFactory("SQL Connection", kernel.Get<ILogger>()));

            var createUserHandler = kernel.Get<CreateUserHandler>();
            createUserHandler.Handle();
        }

        private class MyKernel
        {
            Dictionary<Type, Type> types = new Dictionary<Type, Type>();
            Dictionary<Type, object> instances = new Dictionary<Type, object>();

            public void Register<T1, T2>()
            {
                types[typeof(T1)] = typeof(T2);
            }

            public void Register<T1, T2>(T2 instance)
            {
                instances[typeof(T1)] = instance;
            }

            public T Get<T>()
            {
                return (T)Get(typeof(T));
            }

            public object Get(Type t)
            {
                if (instances.ContainsKey(t))
                    return instances[t];
                
                var resolvedType = types[t];
                var constructor = Utils.GetSingleConstructor(resolvedType);

                var paramsInfo = constructor.GetParameters().ToList();
                var resolvedParams = getParams(paramsInfo);

                var resultObject = Utils.CreateInstance(resolvedType, resolvedParams);
                return resultObject;
            }

            private List<object> getParams(List<ParameterInfo> paramsInfo)
            {
                var resolvedParams = new List<object>();
                foreach (var param in paramsInfo)
                {
                    var type = param.ParameterType;
                    var res = Get(type);
                    resolvedParams.Add(res);
                }
                return resolvedParams;
            }
        }
    }


}