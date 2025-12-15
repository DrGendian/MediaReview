using System.Collections;
using System.Data;
using MediaReview.System;
using Npgsql;
using MediaReview.Model;
namespace MediaReview.Repository;

public interface IRepository
{
    User? Get(object id, Session? session = null);
    IEnumerable GetAll(Session? session = null);
    void Refresh(object obj);
    void Save(object obj);
    void Delete(object obj);
}