namespace DataProviderCollection.CaseInsensitiveDataProvider
{
    using System.Text;
    using Sitecore.Data;
    using Sitecore.Data.DataProviders.Sql;
    using Sitecore.Data.DataProviders.Sql.FastQuery;
    using Sitecore.Data.SqlServer;

    public class CaseInsensitiveSqlDataProvider : SqlDataProvider
    {
        public CaseInsensitiveSqlDataProvider(string connectionString)
            : base(new SqlServerDataApi(connectionString))
        {
        }

        protected override QueryToSqlTranslator CreateSqlTranslator()
        {
            return new CaseInsensitiveQueryToSqlTranslator(this.Api);
        }

        private class CaseInsensitiveQueryToSqlTranslator : QueryToSqlTranslator
        {
            public CaseInsensitiveQueryToSqlTranslator(SqlDataApi api)
                : base(api)
            {
            }

            protected override void AddNameFilter(string name, StringBuilder builder)
            {
                if (ID.IsID(name))
                {
                    builder.Append(this._api.Format("{0}i{1}.{0}ID{1}"));
                }
                else
                {
                    builder.Append(this._api.Format("{0}i{1}.{0}Name{1}"));
                    name = name.ToLowerInvariant();
                }
                builder.Append(" = '" + this._api.Safe(name) + "'");
            }
        }
    }
}
