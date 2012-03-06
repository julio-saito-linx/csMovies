﻿using System;
using System.Collections.Generic;
using Dominio.Entidades;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Utils;
using InfraNhibernate.Convencoes;
using InfraNhibernate.Mappings;
using NHibernate;

namespace InfraNhibernate.NHibernateHelpers
{
    public class SessionFactoryProvider : IDisposable
    {
        private FluentConfiguration _fluentConfiguration;
        private ISessionFactory _sessionFactory;

        #region IDisposable Members

        public void Dispose()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
            }
        }

        #endregion

        public ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory == null)
            {
                _fluentConfiguration = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008.ShowSql()
                                  .ConnectionString(c => c.FromConnectionStringWithKey("ConnectionString")))
                    //.Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Filme>(new AppAutomappingCfg())));
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<FilmeMap>()
                                       .Conventions.Setup(GetConventions()));

                _sessionFactory = _fluentConfiguration.BuildSessionFactory();

                //var cfg = Fluently.Configure()
                //    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Usuario>())
                //    .Database(MsSqlConfiguration.MsSql2008.ShowSql()
                //                  .ConnectionString(c => c.FromConnectionStringWithKey("ConnectionString"))
                //    );
            }

            return _sessionFactory;
        }

        public void AutoCriarBancoDeDados()
        {
            if (_fluentConfiguration != null)
            {
                var sessionSource = new SessionSource(_fluentConfiguration);
                ISession session = sessionSource.CreateSession();
                sessionSource.BuildSchema(session);
            }
        }

        private Action<IConventionFinder> GetConventions()
        {
            return c => c.Add<EnumConvention>();
        }
    }

    public class AppAutomappingCfg : DefaultAutomappingConfiguration
    {
        //IEnumerable<Type> ignoredTypes = new[] {typeof(EntityBase)};

        public override bool ShouldMap(Type type)
        {
            //if (ignoredTypes.Contains(type))
            //    return false;
            return type.Namespace.StartsWith("Dominio.Entidades");
        }

        //public override bool IsDiscriminated(Type type)
        //{
        //    return type == typeof(Person);
        //}

        //public override bool IsComponent(Type type)
        //{
        //    return type.In(typeof(Address), typeof(ContactDetails));
        //}

        //public override bool AbstractClassIsLayerSupertype(Type type)
        //{
        //    return type == tyepof(Client);
        //}
    }
}