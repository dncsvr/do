﻿using Baked.Orm;

namespace Baked.Test.Orm;

public class Parent(IEntityContext<Parent> _context, Func<Child> _newChild, Children _childEntities)
{
    protected Parent()
        : this(default!, default!, default!) { }

    public virtual Guid Id { get; protected set; } = default!;
    public virtual string Name { get; protected set; } = default!;

    public virtual Parent With(string name)
    {
        Name = name;

        return _context.Insert(this);
    }

    public virtual List<Child> GetChildren()
    {
        return _childEntities.ByParent(this);
    }

    public virtual void AddChild()
    {
        _newChild().With(this);
    }

    public virtual void RemoveChild(Child child)
    {
        if (child.Parent != this) { throw new NotMyChildException(child); }

        child.Delete();
    }

    public virtual void Delete()
    {
        foreach (var child in GetChildren())
        {
            child.Delete();
        }
    }
}

public class Parents(IQueryContext<Parent> _context)
{
    public List<Parent> By(
        string? name = default,
        bool asc = false,
        bool desc = false,
        int? take = default,
        int? skip = default
    )
    {
        return
            asc ? _context.By(p => name == default || p.Name == name, orderBy: p => p.Name, take: take, skip: skip) :
            desc ? _context.By(p => name == default || p.Name == name, orderByDescending: p => p.Name, take: take, skip: skip) :
            _context.By(p => name == default || p.Name == name, take: take, skip: skip);
    }

    public List<Parent> ByName(string contains,
        bool asc = false,
        bool desc = false,
        int? take = default,
        int? skip = default
    )
    {
        return
            asc ? _context.By(where: p => p.Name.Contains(contains), orderBy: p => p.Name, take: take, skip: skip) :
            desc ? _context.By(where: p => p.Name.Contains(contains), orderByDescending: p => p.Name, take: take, skip: skip) :
            _context.By(where: p => p.Name.Contains(contains), take: take, skip: skip);
    }
}