using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace AndreyGames.Leaderboards.Service.Implementation
{
    internal sealed class BasicQueryBuilder<TModel>
    {
        private readonly DbContext _dbContext;
        private readonly Dictionary<string, object> _arbitraryParams = new();
        private readonly List<(string Field, object Value)> _ands = new();
        private readonly List<(string Field, object Value1, object Value2)> _andBetweens = new();

        private string _template;
        private string _envelope;
        
        public static string WherePlaceholder => "%%WHERE%%";
        
        private BasicQueryBuilder(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public static BasicQueryBuilder<TModel> New(DbContext dbContext) => new(dbContext);

        public BasicQueryBuilder<TModel> WithQueryTemplate(string template)
        {
            _template = template;
            return this;
        }

        public BasicQueryBuilder<TModel> WithArbitraryParameter(string name, object value)
        {
            _arbitraryParams[name] = value;
            return this;
        }

        public BasicQueryBuilder<TModel> WithEnvelope(string formattedEnvelope)
        {
            _envelope = formattedEnvelope;
            return this;
        }
        
        public BasicQueryBuilder<TModel> And(string field, object value)
        {
            _ands.Add((field, value));
            return this;
        }

        public BasicQueryBuilder<TModel> AndBetween(string field, object value1, object value2)
        {
            _andBetweens.Add((field, value1, value2));
            return this;
        }

        public async Task<IEnumerable<TModel>> Execute()
        {
            var query = BuildQuery();
            var parameters = BuildParameters();

            return await _dbContext.Database.GetDbConnection().QueryAsync<TModel>(query, parameters);
        }

        private string BuildQuery()
        {
            var all = new List<string>();
            
            var ands = string.Join(" AND ",
                _ands.Select(x => x.Field)
                    .Select(x => $"\"{x}\" = @{x}"));

            if (!string.IsNullOrWhiteSpace(ands))
            {
                all.Add(ands);
            }

            var andBetweens = string.Join(" AND ",
                _andBetweens.Select(x => x.Field)
                    .Select(x => $"\"{x}\" BETWEEN @{x}1 AND @{x}2"));

            if (!string.IsNullOrWhiteSpace(andBetweens))
            {
                all.Add(andBetweens);
            }

            var where = string.Join(" AND ", all);

            if (!string.IsNullOrEmpty(_envelope) && !string.IsNullOrWhiteSpace(where))
            {
                where = string.Format(_envelope, where);
            }
            
            return _template.Replace(WherePlaceholder, where);
        }

        private DynamicParameters BuildParameters()
        {
            var dict = new Dictionary<string, object>();
            foreach (var tuple in _ands)
            {
                dict[$"@{tuple.Field}"] = tuple.Value;
            }

            foreach (var tuple in _andBetweens)
            {
                dict[$"@{tuple.Field}1"] = tuple.Value1;
                dict[$"@{tuple.Field}2"] = tuple.Value2;
            }

            foreach (var param in _arbitraryParams)
            {
                dict[$"@{param.Key}"] = param.Value;
            }

            return new DynamicParameters(dict);
        }
    }
}