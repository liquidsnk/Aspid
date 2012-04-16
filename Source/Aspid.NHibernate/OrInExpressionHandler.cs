#region License
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Aspid.Core;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace Aspid.NHibernate
{

    /// <summary>
    /// Special In Expression that handle maximum indentifiers. Note: This class could become an extension to Hibernate.
    /// </summary>
    public class OrInExpressionHandler : AbstractCriterion
    {
        const int MAX_VALUES_PER_IN_QUERY = 1000;

        public ICriterion Criterion { get; set; }

        public static ICriterion GetCriterion<T>(Expression<Func<T, object>> property, ICollection values)
        {
            return GetCriterion(property, values, MAX_VALUES_PER_IN_QUERY);
        }

        public static ICriterion GetCriterion<T>(Expression<Func<T, object>> property, ICollection values, int maxValuesPerInQuery)
        {
            return GetCriterion(Reflect<T>.PropertyName(property), values, maxValuesPerInQuery);
        }

        public static ICriterion GetCriterion(string propertyName, ICollection values)
        {
            return GetCriterion(propertyName, values, MAX_VALUES_PER_IN_QUERY);
        }

        public static ICriterion GetCriterion(string propertyName, ICollection values, int maxValuesPerInQuery)
        {
            return GetCriterion(Projections.Property(propertyName), values, maxValuesPerInQuery);
        }

        public static ICriterion GetCriterion(IProjection projection, ICollection values)
        {
            return GetCriterion(projection, values, MAX_VALUES_PER_IN_QUERY);
        }

        public static ICriterion GetCriterion(IProjection projection, ICollection values, int maxValuesPerInQuery)
        {
            var criterionMaker = new OrInExpressionHandler(projection, values, maxValuesPerInQuery);
            return criterionMaker.Criterion;
        }

        protected OrInExpressionHandler(IProjection projection, ICollection values, int maxValuesPerInQuery)
        {
            Criterion = BuildSingleInCriterion(projection, new ArrayList(values), maxValuesPerInQuery);
        }

        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return Criterion.GetTypedValues(criteria, criteriaQuery);
        }

        /// <summary>
        /// Recursive method that will create criterion that will have inner criterion with OR IN statement
        /// </summary>
        /// <param name="values"></param>
        /// <param name="lhs"></param>
        /// <param name="startIndex"></param>
        /// <param name="recordSize"></param>
        /// <returns></returns>
        private static ICriterion BuildSingleInCriterion(IProjection property, ArrayList values, int maxValuesPerInQuery)
        {
            if (values.Count <= maxValuesPerInQuery) return Restrictions.In(property, values);

            return Restrictions.Or(Restrictions.In(property, values.GetRange(0, maxValuesPerInQuery)),
                                   BuildSingleInCriterion(property, values.GetRange(maxValuesPerInQuery, values.Count - maxValuesPerInQuery), maxValuesPerInQuery));
        }

        public override IProjection[] GetProjections()
        {
            return Criterion.GetProjections();
        }

        public override string ToString()
        {
            return Criterion.ToString();
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
        {
            return Criterion.ToSqlString(criteria, criteriaQuery, enabledFilters);
        }
    }
}
