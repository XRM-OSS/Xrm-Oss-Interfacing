using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MassTransit;
using Xrm.Oss.Interfacing.Domain.Contracts;

namespace Xrm.Oss.Interfacing.CrmListener
{
    public class Container
    {
        private static IWindsorContainer _instance;

        private Container() { }

        public static IWindsorContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WindsorContainer();
                    _instance.Install(FromAssembly.This());
                }

                return _instance;
            }
        }
    }
}