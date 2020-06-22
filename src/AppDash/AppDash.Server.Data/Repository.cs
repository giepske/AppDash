﻿using System.Collections.Generic;
using System.Linq;
using AppDash.Server.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace AppDash.Server.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext _context;

        private DbSet<TEntity> _entities;

        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        protected virtual DbSet<TEntity> Entities => _entities ??= _context.Set<TEntity>();

        public Repository(DbContext context)
        {
            _context = context;
        }

        public TEntity GetById(object id)
        {
            return Entities.Find(id);
        }

        public TEntity Insert(TEntity entity)
        {
            entity = Entities.Add(entity).Entity;

            _context.SaveChanges();

            return entity;
        }

        /// <summary>
        /// Insert the entity without tracking it, this should be used as little as possible and only exists for unit testing.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity InsertNoTracking(TEntity entity)
        {
            entity = Entities.Add(entity).Entity;

            _context.SaveChanges();

            _context.Entry(entity).State = EntityState.Detached;

            return entity;
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
            Entities.AddRange(entities);

            _context.SaveChanges();
        }

        public TEntity Update(TEntity entity)
        {
            entity = Entities.Update(entity).Entity;

            _context.SaveChanges();

            return entity;
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            Entities.UpdateRange(entities);

            _context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            Entities.Remove(entity);

            _context.SaveChanges();
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            Entities.RemoveRange(entities);

            _context.SaveChanges();
        }
    }
}