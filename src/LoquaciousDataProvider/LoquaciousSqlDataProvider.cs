namespace DataProviderCollection.LoquaciousDataProvider
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sitecore.Collections;
    using Sitecore.Data.DataProviders;
    using Sitecore.Data.DataProviders.Sql;
    using Sitecore.Data.DataProviders.Sql.FastQuery;
    using Sitecore.Data.SqlServer;
    using Sitecore.Diagnostics;

    /// <summary>
    /// A very loquacious data provider.
    /// </summary>
    public class LoquaciousSqlDataProvider : SqlDataProvider
    {
        /// <summary>
        /// The filters.
        /// </summary>
        private readonly List<string> filters = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoquaciousSqlDataProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public LoquaciousSqlDataProvider(string connectionString)
            : base(new SqlServerDataApi(connectionString))
        {
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public List<string> Filters
        {
            get
            {
                return this.filters;
            }
        }

        /// <summary>
        /// Basically call the standard data provider.
        /// Queries the fast.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="context">The context.</param>
        /// <returns>A list of IDs.</returns>
        protected override IDList QueryFast(string query, CallContext context)
        {
            var baseIdList = this.SelectIDs(query, context);
            if (baseIdList != null && baseIdList.Count > 0) return baseIdList;

            if (!this.IsTraceEnabled(query)) return base.QueryFast(query, context);

            var parameters = new ParametersList();
            var sql = this.Translator.TranslateQuery(query, context, parameters);

            Log.Debug(string.Format("FastQuery: {0}", query), this);
            Log.Debug(string.Format("SQL Query: {0}", this.FormatSqlQuery(sql, parameters)), this);

            if (sql == null) return null;

            var stopwatch = Stopwatch.StartNew();
            using (var reader = this.Api.CreateReader(sql, parameters.ToArray()))
            {
                var idList = new IDList();
                while (reader.Read())
                {
                    idList.Add(this.Api.GetId(0, reader));
                }

                context.CurrentResult = idList;
            }

            Log.Debug(string.Format("Query Time: {0}ms", stopwatch.ElapsedMilliseconds), this);
            return null;
        }

        /// <summary>
        /// Determines whether the trace is enabled for the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>True, if enabled.</returns>
        private bool IsTraceEnabled(string query)
        {
            return this.Filters.Count == 0 || this.Filters.Any(query.Contains);
        }

        /// <summary>
        /// Formats the SQL query by replacing the parameters with the actual values.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameterList">The parameter list.</param>
        /// <returns>The formatted query.</returns>
        private string FormatSqlQuery(string sql, ParametersList parameterList)
        {
            var parameters = parameterList.ToArray();
            for (var i = 0; i < parameters.Length; i = i + 2)
            {
                var parameterName = string.Format("@{0}", parameters[i]);
                var parameterValue = string.Format("'{0}'", parameters[i + 1]);

                sql = sql.Replace(parameterName, parameterValue);
            }

            return sql;
        }
    }
}
