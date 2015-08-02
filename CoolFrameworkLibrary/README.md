# CoolFramework

A C#.NET library for Object Oriented containers and publish-subscribe
inter-component middleware.  This has been built to allow a more abstract
level of composability based on role that has a more loosely defined
contract than interfaces or inheritance provide.

Containers

* Bunch         - An unsorted collection.

* Groups        - key-multi-value-pairs.  A group is identified with a key and can hold many items.

* UniqueSet     - A set of unique objects.

* AvlTree       - A binary tree that rebalances on insertion (maintaining the AVL property).


Middleware

* MessageBus

* Message

* MessageSubscriber


#Unity3D

This library is designed for use with Unity3D and underpins Lokel Digital's
editor extension products.

Download the source or clone the GIT repository under the Asset folder in
your Unity project.

#VisualStudio

This library was built with VisualStudio but uses the nUnit test framework
to make it most accessible.  Therefore, it could also be developed with
Xamarin or other .NET IDE tools.

#Licence and Copyright

Sponsor: Lokel Digital Pty Ltd (Melbourne)

For more up-to-date documentation, [see the GitHub
project wiki](https://github.com/lokeldigital/CoolFramework/wiki).

Licence and Copyright
---------------------

Copyright (C) 2014, 2015 Lokel Digital Pty Ltd

website: http://www.lokeldigital.com    

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
USA
