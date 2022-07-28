# QueryObjectFilter
Provides an abstraction for filtering query results, based on the [QueryObject](https://www.martinfowler.com/eaaCatalog/queryObject.html) pattern.

Use it in CQRS query handlers and QueryObject to filter query results by query, e.g. to simplify the logic of filtering UI datatable by certain fields.

**QueryObjectFilter** - filtering criteria, used on the Application layer.
**QueryObjectFilter.Conversion** - converters of filtering criteria into a certain format (converters are implemented in Expression and SQL), used on the Persistence layer.
**QueryObjectFilter.DI.MicrosoftDependencyInjection** - extension for dependency injection.

### QueryObjectFiltration
The main class to use - **FilterCriteria<TSource, TFilter>** - is a collection of grouped filter criteria.
**TSource** - filtered object type (query result type).
**TFilter** - filter type (query type).

FilterCriteria contains a list of criteria groups. Each group is a list of criteria grouped by one logical operation (AND or OR). Groups can be nested within other groups, i.e. various logical combinations are realized.
The criterion incdude the filtered object, the filtering value and the comparison method. Filtering by criteria compares the value of the specified property of the filtered type with the specified filter value by the specified method.

If the filter value is null, then the criterion will not be created. I.e. if some fields are not assigned (null) in the query, will be no filtering by them.

Generally, the FilterCriteria are built in a CQRS query handler and passed to a QueryObject whose implementation uses a specific converter to convert the criteria into a specific conditional expression (Expression or SQL).

```csharp
//Query result   
public class UserProjection
{
    public long Id { get; set; }
 
    public string Email { get; set; }
 
    public string FirstName { get; set; }
 
    public string MiddleName { get; set; }
 
    public string LastName { get; set; }
 
    public string ActiveDirectoryName { get; set; }
 
    public List<LoginData> Logins { get; set; }
 
    public int Status { get; set; }
}
 
public class LoginData
{
    public string LoginValue { get; set; }
 
    public string LoginProviderName { get; set; }
}
 
//Query
public class GetUserQuery : IQuery<GetUserResponse>
{
    public long? Id { get; set; } //all properties is are nullable
 
    public string Email { get; set; }
 
    public string Name { get; set; }
 
    public string ActiveDirectoryName { get; set; }
 
    public string Login { get; set; }
 
    public string LoginProviderName { get; set; }
 
    public List<int> Statuses { get; set; } //collection of possible values
 
    public int? Skip { get; set; } //additional data not related to filtering
 
    public int? Take { get; set; }
}
```
```csharp
var filter = new FilterCriteria<UserProjection, GetUserQuery>(query); //create FilterCriteria
filter
    .AndGroup() //group of criteria combined via AND
        .AddCriteria(u => u.Id, q => q.Id, CompareMethod.GreaterThan)
        .AddCriteria(u => u.Email, q => q.Email, CompareMethod.Equal)
        .OrGroup() //nested group of criteria combined via OR
            .AddCriteria(u => u.LastName, q => q.Name, CompareMethod.StartsWith)
            .AddCriteria(u => u.FirstName, q => q.Name, CompareMethod.Contains)
            .AddCriteria(u => u.MiddleName, q => q.Name, CompareMethod.Contains)
            .CloseGroup() //nested groups must be closed if you want to add criteria to the parent group
        .AddCriteria(u => u.ActiveDirectoryName, q => q.ActiveDirectoryName, CompareMethod.Equal)
        .OrGroup()
            .AddCriteria(r => r.Logins, login => login.LoginValue, q => q.Login, CompareMethod.Equal) //criteria for a nested collection, you need to specify the collection and the property of the collection element
            .AddCriteria(r => r.Logins, login => login.LoginProviderName, q => q.LoginProviderName, CompareMethod.Equal) //separate criteria is created for each property of the collection element
            .CloseGroup()
        .AddCriteria(u => u.Status, q => q.Statuses, CompareMethod.In); //criteria on a list of values
 
//abstraction over condition
//     u.Id > q.Id
// AND u.Email == q.Email
// AND (
//      u.LastName.StartWith(q.Name)
//   OR u.FirstName.Contains(q.Name)
//   OR u.MiddleName.Contains(q.Name)
//     )
// AND u.ActiveDirectoryName == q.ActiveDirectoryName
// AND (
//      u.Logins.Any(l => l.LoginValue == q.Login
//                     OR l.LoginProviderName == q.LoginProviderName)
//     )
// AND q.Statuses.Contains(u.Status)
```

## Using converters
The main converter interface - **IFilterCriteriaConverter<T>** - contains the method
```csharp
T Convert<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria, string   parameterName = null)
```
The result of the conversion can be any type, Expression and string (SQL) are implemented. To implement your type, you must implement the **ICompareMethodProvider\<T>** and **IConverterProvider\<T>** interfaces.

**FilterCriteriaExpressionConverter** - converter to Expression, with additional method 
```csharp
Expression<Func<TSource, bool>> GetExpression<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria)
```
**FilterCriteriaToSqlConverter** - converter to SQL, with additional method
```csharp
string GetSql<TSource, TFilter>(FilterCriteria<TSource, TFilter> filterCriteria)
```
Each class has its own specific interface - **IFilterCriteriaExpressionConverter** and **IFilterCriteriaSqlConverter**, for convenience, it is recommended to use them instead of **IFilterCriteriaConverter\<T>**.

EF Core example:
```csharp
public class GetUserProjectionQueryObject : IGetUserProjectionQueryObject
{
    private readonly IdentityContext identityContext; //DBContext EF Core
    private readonly IFilterCriteriaExpressionConverter filterCriteriaExpressionConverter; //FilterCriteria to Expression converter
 
    public GetUserProjectionQueryObject(IdentityContext identityContext, IFilterCriteriaExpressionConverter filterCriteriaExpressionConverter)
    {
        this.identityContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));
        this.filterCriteriaExpressionConverter = filterCriteriaExpressionConverter ?? throw new ArgumentNullException(nameof(filterCriteriaExpressionConverter));
    }
 
    public async Task<IList<UserProjection>> GetAsync(FilterCriteria<UserProjection, GetUserQuery> filterCriteria, int? skip = null, int? take = null)
    {
        var users = from user in identityContext.Users.OrderBy(u => u.Id)
                    select new UserProjection()
                    {
                        Id = user.Id,
                        Email = user.Email.Value,
                        FirstName = user.PersonalData.FirstName,
                        LastName = user.PersonalData.LastName,
                        MiddleName = user.PersonalData.MiddleName,
                        ActiveDirectoryName = user.Logins.FirstOrDefault(x => x.Provider.Value == LoginProvider.Windows.Value).Value,
                        Logins = user.Logins.Select(l => new LoginData() { LoginValue = l.Value, LoginProviderName = l.Provider.Name }).ToList(),
                        Status = user.Status.Value
                    };
 
        //filtering
        users = users.Where(filterCriteriaConverter.GetExpression(filterCriteria)); //use GetExpression()
 
        //pagination by additional query parameters
        if (skip != null)
            users = users.Skip(skip.Value);
 
        if (take != null)
            users = users.Take(take.Value);
 
        return await users.ToListAsync();
    }
}
```
Dapper example:
```csharp
public class GetUserProjectionQueryObject : IGetUserProjectionQueryObject
{
    private readonly IdentityConnection connection;
    private readonly IFilterCriteriaSqlConverter filterCriteriaSqlConverter; //FilterCriteria to SQL converter
 
    public GetUserProjectionQueryObject(IdentityConnection connection, IFilterCriteriaSqlConverter filterCriteriaSqlConverter)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.filterCriteriaSqlConverter = filterCriteriaSqlConverter ?? throw new ArgumentNullException(nameof(filterCriteriaSqlConverter));
    }
 
    public async Task<IList<UserProjection>> GetAsync(FilterCriteria<UserProjection, GetUserQuery> filterCriteria, int? skip = null, int? take = null)
    {
        var query = $@"
WITH Projection AS
(
SELECT u.Id, u.Email, u.PersonalData_LastName LastName, u.PersonalData_FirstName FirstName, u.PersonalData_MiddleName MiddleName,
       lAd.Value ActiveDirectoryName,
       l.Value LoginValue, l.Provider_Name LoginProviderName,
       u.Status
FROM [User] u LEFT JOIN Login l ON u.Id = l.UserId
              LEFT JOIN Login lAd ON u.Id = lAd.UserId AND lAd.Provider_Value = 1
)
 
SELECT *
FROM Projection
WHERE ({filterCriteriaConverter.GetSql(filterCriteria)})"; //use GetSql()
 
        using var connection = this.connection.CreateConnection();
        return (await connection.QueryAsync<UserProjection>(query)).ToList();
    }
}
```

### DI extensions
Extensions for MicrosoftDependencyInjection are implemented.
```csharp
services.AddQueryObjectFilterWithExpressionConversion();
services.AddQueryObjectFilterWithSqlConversion();
```







