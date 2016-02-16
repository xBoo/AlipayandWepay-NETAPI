using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AW.Pay.Core;
using AW.Pay.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AW.Webapi.Sample.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            SetAutoFacContainer();
        }

        private static void SetAutoFacContainer()
        {
            var builder = new ContainerBuilder();

            //Get HttpConfiguration
            var config = GlobalConfiguration.Configuration;

            //Register MVC Controller
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            //Register Web Api Controller
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            //Register Pay Interface
            builder.RegisterType<AliPay>().As<IAlipay>().InstancePerRequest();
            builder.RegisterType<WePay>().As<IWePay>().InstancePerRequest();

            //Set the dependency resolver to be Autofac
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container); //Web Api
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); //MVC

        }
    }
}