/*
 *  CoolFramework
 *  -------------
 *  A C#.NET library for Object Oriented containers and publish-subscribe
 *  inter-component middleware.  This has been built to allow a more abstract
 *  level of composability based on role that has a more loosely defined
 *  contract than interfaces or inheritance provide.
 *  
 *  Copyright (C) 2014, 2015 Lokel Digital Pty Ltd
 *  
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 *  USA
 */


using System;
using System.Collections.Generic;
using System.Text;


namespace Lokel.CoolFramework {

    public interface R_Router : R_Role { }

    public interface R_Controller : R_Role {

    }

    public interface R_MetaModel : R_Role {
        /// <summary>
        /// Unique instance ID that can be persisted and used to reconstruct
        /// references to this object, from other objects.
        /// </summary>
        /// <returns>int ID code - always valid.</returns>
        int GetID();
    }
    public interface R_ModelFactory : R_Role { }
    public interface R_Model : R_Role {
        /// <summary>
        /// Unique instance ID that can be persisted and used to reconstruct
        /// references to this object, from other objects.
        /// </summary>
        /// <returns>int ID code - always valid.</returns>
        int GetID();
    }

    // A composite view
    public interface R_ViewPort : R_Role { }
    public interface R_View : R_Role { }

    public interface R_Renderer : R_Role {
    }

    public interface R_Palette : R_Role { }

    public enum ViewPortEvents {
        VP_ModelCreated,
        VP_ModelDestroyed
    }

    public enum ViewEvents {
        VE_ModelChangedData,
        VE_ModelChangedVisually,
        VE_MouseButton_Down,
        VE_MouseButton_Up,
        VE_MouseMoved
    }

    public enum MetaModelEvents {
        MM_New, //(string name);
        MM_Opened, //(string name);
        MM_Saved,
        MM_Closed
    }

    public enum ModelFactoryEvents {
        MF_Created,
        MF_DestroyingAll
    }

    public enum ModelEvents {
        MO_Changed,
        MO_Moved,
        MO_Destroyed
    }

    public enum ControllerEvents {
        CE_MouseButton_Down_OnView,
        CE_MouseButton_Up_OnView,
        CE_MouseMoved_NearView
    }

} //--namespace--
