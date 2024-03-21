using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

// I am not entering heaven

public static class DarkProgramming
{
    /*
        // Pointer to an external unmanaged resource.
        private static unsafe Collision* handle;
        // Track whether Dispose has been called.
        private static bool disposed = false;
        public static List<Tuple<Collision,bool>> collisionList;

        public static void MarkCreation(this Collision collision) {
            collisionList.Add(new Tuple<Collision, bool>(collision,false));
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public static unsafe void Dispose(this Collision collision)
        {
            collision.Dispose(disposing: true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(collision);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        static unsafe void Dispose(this Collision collision, bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!collision.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    collision.contacts.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                CloseHandle(handle);
                handle = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        // Use C# finalizer syntax for finalization code.
        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide finalizer in types derived from this class.
        ~Collision(this Collision collision)
        {
            Dispose(disposing: false);
        }
        */
}
