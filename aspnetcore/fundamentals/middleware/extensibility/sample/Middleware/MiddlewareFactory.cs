using System;
using Microsoft.AspNetCore.Http;
using MiddlewareExtensibilitySample.Data;

namespace MiddlewareExtensibilitySample.Middleware
{
    #region snippet1
    public class BasicMiddlewareFactory : IMiddlewareFactory
    {
        private readonly AppDbContext _db;

        public BasicMiddlewareFactory(AppDbContext db)
        {
            _db = db;
        }

        public IMiddleware Created { get; private set; }
        public IMiddleware Released { get; private set; }

        public IMiddleware Create(Type middlewareType)
        {
            Created = Activator.CreateInstance(middlewareType, _db) as IMiddleware;

            return Created;
        }

        public void Release(IMiddleware middleware)
        {
            Released = middleware;
        }
    }
    #endregion
}
