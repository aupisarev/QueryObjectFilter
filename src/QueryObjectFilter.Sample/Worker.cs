using QueryObjectFilter.Conversion.ToExpression;
using QueryObjectFilter.Conversion.ToSql;
using QueryObjectFilter.Converting;
using QueryObjectFilter.Filtration;
using System.Linq;
using System.Linq.Expressions;

namespace QueryObjectFilter.Sample
{
    public class Worker : BackgroundService
    {
        private readonly IFilterCriteriaExpressionConverter filterCriteriaExpressionConverter;
        private readonly IFilterCriteriaSqlConverter filterCriteriaSqlConverter;

        public Worker(IFilterCriteriaExpressionConverter filterCriteriaExpressionConverter, IFilterCriteriaSqlConverter filterCriteriaSqlConverter)
        {
            this.filterCriteriaExpressionConverter = filterCriteriaExpressionConverter ?? throw new ArgumentNullException(nameof(filterCriteriaExpressionConverter));
            this.filterCriteriaSqlConverter = filterCriteriaSqlConverter ?? throw new ArgumentNullException(nameof(filterCriteriaSqlConverter));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var users = new List<UserProjection>()
            {
                new UserProjection(){ Id = 1, Email = "u1@user.com", LastName = "Ivanov", FirstName = "Ivan", MiddleName = "Ivanovich", ActiveDirectoryName = "u1", Logins = new(){new LoginData() { LoginProviderName = "IS1", LoginValue = "u1"} }, Status = 1},
                new UserProjection(){ Id = 2, Email = "u2@user.com", LastName = "Petrov", FirstName = "Petr", MiddleName = "Petrovich", ActiveDirectoryName = "u2", Logins = new(){new LoginData() { LoginProviderName = "IS2", LoginValue = "u2"} }, Status = 1},
                new UserProjection(){ Id = 3, Email = "u3@user.com", LastName = "Sidorov", FirstName = "Sergey", MiddleName = "Sergeevich", ActiveDirectoryName = "u3", Logins = new(){new LoginData() { LoginProviderName = "IS1", LoginValue = "u3"} }, Status = 2},
                new UserProjection(){ Id = 4, Email = "u4@user.com", LastName = "Popov", FirstName = "Nikolay", MiddleName = "Nikolaevich", ActiveDirectoryName = "u4", Logins = new(){new LoginData() { LoginProviderName = "IS2", LoginValue = "u4"} }, Status = 1},
                new UserProjection(){ Id = 5, Email = "u5@user.com", LastName = "Sokolov", FirstName = "Ivan", MiddleName = "Ivanovich", ActiveDirectoryName = "u5", Logins = new(){new LoginData() { LoginProviderName = "IS1", LoginValue = "u5"} }, Status = 3},
            };

            var query = new GetUserQuery()
            {
                Id = 1,
                Login = "u3",
                LoginProviderName = "IS1",
                Statuses = new List<int> { 1, 2, 3 }
            };

            var filter = new FilterCriteria<UserProjection, GetUserQuery>(query); //создание объекта фильтра
            filter
                .AndGroup() //группа критериев, скомбинированных через AND
                    .AddCriteria(u => u.Id, q => q.Id, CompareMethod.GreaterThan)
                    .AddCriteria(u => u.Email, q => q.Email, CompareMethod.Equal)
                    .OrGroup() //вложенная группа критериев, скомбинированных через OR
                        .AddCriteria(u => u.LastName, q => q.Name, CompareMethod.StartsWith)
                        .AddCriteria(u => u.FirstName, q => q.Name, CompareMethod.Contains)
                        .AddCriteria(u => u.MiddleName, q => q.Name, CompareMethod.Contains)
                        .CloseGroup() //вложенные группы необходимо закрывать, если необходимо добавить критерии на родительскую группу
                    .AddCriteria(u => u.ActiveDirectoryName, q => q.ActiveDirectoryName, CompareMethod.Equal)
                    .OrGroup()
                        .AddCriteria(r => r.Logins, login => login.LoginValue, q => q.Login, CompareMethod.Equal) //указывается коллекция и свойство элемента коллекции
                        .AddCriteria(r => r.Logins, login => login.LoginProviderName, q => q.LoginProviderName, CompareMethod.Equal) //на каждое свойство элемента коллекции создается отдельный критерий
                        .CloseGroup()
                    .AddCriteria(u => u.Status, q => q.Statuses, CompareMethod.In); //критерий по списку значений

            var expression = filterCriteriaExpressionConverter.GetExpression(filter);
            var expressionResult = users.AsQueryable().Where(expression).ToList();

            var sql = filterCriteriaSqlConverter.GetSql(filter);

            return Task.CompletedTask;
        }
    }
}