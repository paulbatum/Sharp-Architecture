﻿using System.Web.Mvc;
using SharpArch.Data.NHibernate;
using System;

namespace SharpArch.Web.NHibernate
{
	public class TransactionAttribute : ActionFilterAttribute
	{
        /// <summary>
        /// When used, assumes the <see cref="factoryKey" /> to be NHibernateSession.DefaultFactoryKey
        /// </summary>
        public TransactionAttribute() { }

        /// <summary>
        /// Overrides the default <see cref="factoryKey" /> with a specific factory key
        /// </summary>
        public TransactionAttribute(string factoryKey) {
            this.factoryKey = factoryKey;
	    }

		public override void OnActionExecuting(ActionExecutingContext filterContext) {
            NHibernateSession.CurrentFor(GetEffectiveFactoryKey()).BeginTransaction();
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext) {
            string effectiveFactoryKey = GetEffectiveFactoryKey();

            if (filterContext.Exception == null && NHibernateSession.CurrentFor(effectiveFactoryKey).Transaction.IsActive) {
                NHibernateSession.CurrentFor(effectiveFactoryKey).Transaction.Commit();
            }
		}

        private string GetEffectiveFactoryKey() {
            return String.IsNullOrEmpty(factoryKey)
                    ? NHibernateSession.DefaultFactoryKey
                    : factoryKey;
        }

        /// <summary>
        /// Optionally holds the factory key to be used when beginning/committing a transaction
        /// </summary>
        private string factoryKey;
	}
}
