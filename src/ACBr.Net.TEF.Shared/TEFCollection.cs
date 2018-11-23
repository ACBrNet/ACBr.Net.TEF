// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="TEFCollection.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2016 Grupo ACBr.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Generics;

namespace ACBr.Net.TEF
{
    [Serializable]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class TEFCollection<TTipo> : GenericClone<TTipo>, IEnumerable<TTipo> where TTipo : class
    {
        #region Fields

        protected List<TTipo> List;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFCollection{TTipo}"/> class.
        /// </summary>
        internal TEFCollection()
        {
            List = new List<TTipo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFCollection{TTipo}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        internal TEFCollection(IEnumerable<TTipo> source)
        {
            List = new List<TTipo>(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFCollection{TTipo}"/> class.
        /// </summary>
        internal TEFCollection(int capacity)
        {
            List = new List<TTipo>(capacity);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds an object to the end of the <see cref="TEFCollection{TTipo}"/>.
        /// </summary>
        /// <returns>T.</returns>
        internal TTipo AddNew()
        {
            var item = (TTipo)Activator.CreateInstance(typeof(TTipo), true);
            List.Add(item);
            return item;
        }

        /// <summary>Adds an object to the end of the <see cref="TEFCollection{TTipo}"/>.</summary>
        /// <param name="item">The object to be added to the end of the <see cref="TEFCollection{TTipo}"/>. The value can be null for reference types.</param>
        internal void Add(TTipo item)
        {
            List.Add(item);
        }

        internal void AddRange(IEnumerable<TTipo> itens)
        {
            List.AddRange(itens);
        }

        /// <summary>Inserts an element into the <see cref="TEFCollection{TTipo}"/> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is greater than <see cref="TEFCollection{TTipo}.Count"/>.</exception>
        internal void Insert(int index, TTipo item)
        {
            List.Insert(index, item);
        }

        /// <summary>Inserts the elements of a collection into the <see cref="TEFCollection{TTipo}"/> at the specified index.</summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="TEFCollection{TTipo}"/>. The collection itself cannot be null, but it can contain elements that are null, if type <paramref name="T" /> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is greater than <see cref="TEFCollection{TTipo}.Count"/>.</exception>
        internal void InsertRange(int index, IEnumerable<TTipo> collection)
        {
            List.InsertRange(index, collection);
        }

        internal void Clear()
        {
            List.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TTipo> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Methods

        #region Properties

        /// <summary>Gets the count.</summary>
        /// <value>The count.</value>
        public int Count => List.Count;

        /// <summary>
        ///
        /// </summary>
        /// <param name="idx"></param>
        public TTipo this[int idx]
        {
            get
            {
                Guard.Against<IndexOutOfRangeException>(idx >= Count || idx < 0);
                return List[idx];
            }
            set
            {
                Guard.Against<IndexOutOfRangeException>(idx >= Count || idx < 0);
                List[idx] = value;
            }
        }

        #endregion Properties

        #region Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:TTipo[]"/> to <see cref="TEFCollection{TTipo}"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TEFCollection<TTipo>(TTipo[] source) => new TEFCollection<TTipo>(source);

        #endregion Operators
    }
}