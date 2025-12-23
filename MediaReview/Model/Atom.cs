using MediaReview.System;
﻿using MediaReview.Repository;

namespace MediaReview.Model
{
    public abstract class Atom: IAtom
    {
        protected Session? _EditingSession = null;
        protected abstract IRepository _GetRepository();
        
        protected void _VerifySession(Session? session = null)
        {
            if(session is not null) { _EditingSession = session; }
            if(_EditingSession is null && !_EditingSession.Valid) { throw new UnauthorizedAccessException("Invalid session."); }
        }
        
        protected void _EndEdit()
        {
            _EditingSession = null;
        }
        
        protected void _EnsureAdmin()
        {
            _VerifySession();
            if(!_EditingSession!.IsAdmin) { throw new UnauthorizedAccessException("Admin privileges required."); }
        }
        
        protected void _EnsureAdminOrOwner(string owner)
        {
            _VerifySession();
            if(!(_EditingSession!.IsAdmin || (_EditingSession.UserName == owner)))
            {
                throw new UnauthorizedAccessException("Admin or owner privileges required.");
            }
        }
        
        protected void _EnsureAdminOrOwner(int userId)
        {
            _VerifySession();
            if(!(_EditingSession!.IsAdmin || (_EditingSession.UserId == userId)))
            {
                throw new UnauthorizedAccessException("Admin or owner privileges required.");
            }
        }
        
        public virtual void BeginEdit(Session session)
        {
            _VerifySession(session);
        }
        
        public virtual void Save()
        {
            _GetRepository().Save(this);
            _EndEdit();
        }
        
        public virtual void Delete()
        {
            _GetRepository().Delete(this);
            _EndEdit();
        }
        
        public virtual void Refresh()
        {
            _GetRepository().Refresh(this);
            _EndEdit();
        }
    }
}

