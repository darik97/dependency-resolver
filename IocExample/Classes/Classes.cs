using System;

namespace IocExample.Classes
{
    public class RestClient
    {
        public RestClient(string apiKey)
        {

        }
    }

    public class CacheService
    {
        private readonly ILogger consoleLogger;
        private readonly RestClient restClient;

        public CacheService(ILogger consoleLogger, RestClient restClient)
        {
            this.consoleLogger = consoleLogger;
            this.restClient = restClient;
        }
    }

    public class CommandExecutor
    {
        private readonly IConnectionFactory connectionFactory;

        public CommandExecutor(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
    }

    public interface IConnectionFactory
    {

    }

    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly ILogger logger;

        public SqlConnectionFactory(string sqlConnection, ILogger logger)
        {
            this.logger = logger;
            logger.Write(string.Format("SqlConnectionFactory was created {0}", sqlConnection));
        }
    }

    public class PgSqlConnectionFactory : IConnectionFactory
    {
    }

    public class QueryExecutor
    {
        private IConnectionFactory connectionFactory;

        public QueryExecutor(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public interface ILogger
    {
        void Write(string message);
    }

    public class UserService
    {
        private readonly QueryExecutor queryExecutor;
        private readonly CommandExecutor commandExecutor;

        public UserService(QueryExecutor queryExecutor, CommandExecutor commandExecutor, CacheService cacheService)
        {
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
        }
    }

    public class CreateUserHandler
    {
        private ILogger logger;
        private UserService userService;

        public CreateUserHandler(UserService userService, ILogger logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        public void Handle()
        {
            logger.Write("User handler was invoked!");
        }
    }
}