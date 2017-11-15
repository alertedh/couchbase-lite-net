﻿// 
// ReplicatorConfiguration.cs
// 
// Author:
//     Jim Borden  <jim.borden@couchbase.com>
// 
// Copyright (c) 2017 Couchbase, Inc All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Couchbase.Lite.Sync
{
    /// <summary>
    /// An enum representing the direction of a <see cref="Replicator"/>
    /// </summary>
    [Flags]
    public enum ReplicatorType
    {
        /// <summary>
        /// The replication will push data from local to remote
        /// </summary>
        Push = 1 << 0,

        /// <summary>
        /// The replication will pull data from remote to local
        /// </summary>
        Pull = 1 << 1,

        /// <summary>
        /// The replication will operate in both directions
        /// </summary>
        PushAndPull = Push | Pull
    }

    /// <summary>
    /// A class representing configuration options for a <see cref="Replicator"/>
    /// </summary>
    public sealed class ReplicatorConfiguration
    {
        /// <summary>
        /// Gets or sets the local database participating in the replication. 
        /// </summary>
        public Database Database { get; }

        /// <summary>
        /// Gets the target to replicate with (either <see cref="Database"/>
        /// or <see cref="Uri"/>
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// A value indicating the direction of the replication.  The default is
        /// <see cref="ReplicatorType.PushAndPull"/> which is bidirectional
        /// </summary>
        public ReplicatorType ReplicatorType { get; set; } = ReplicatorType.PushAndPull;

        /// <summary>
        /// Gets or sets whether or not the <see cref="Replicator"/> should stay
        /// active indefinitely.  The default is <c>false</c>
        /// </summary>
        public bool Continuous { get; set; }

        /// <summary>
        /// Gets or sets the object to use when resolving incoming conflicts.  The default
        /// is <c>null</c> which will set up the default algorithm of the most active revision
        /// </summary>
        public IConflictResolver ConflictResolver { get; set; }

        /// <summary>
        /// Gets or sets a certificate to trust.  All other certificates received
        /// by a <see cref="Replicator"/> with this configuration will be rejected.
        /// </summary>
        public X509Certificate2 PinnedServerCertificate
        {
            get => Options.PinnedServerCertificate;
            set => Options.PinnedServerCertificate = value;
        }

        /// <summary>
        /// Extra HTTP headers to send in all requests to the remote target
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get => Options.Headers;
            set => Options.Headers = value;
        }

        /// <summary>
        /// A set of Sync Gateway channel names to pull from.  Ignored for push replicatoin.
        /// The default value is null, meaning that all accessible channels will be pulled.
        /// Note: channels that are not accessible to the user will be ignored by Sync Gateway.
        /// </summary>
        public IList<string> Channels
        {
            get => Options.Channels;
            set => Options.Channels = value;
        }

        /// <summary>
        /// A set of document IDs to filter by.  If not null, only documents with these IDs will be pushed
        /// and/or pulled
        /// </summary>
        public IList<string> DocumentIDs
        {
            get => Options.DocIDs;
            set => Options.DocIDs = value;
        }

        /// <summary>
        /// Gets or sets the class which will authenticate the replication
        /// </summary>
        public Authenticator Authenticator { get; set; }

        internal ReplicatorOptionsDictionary Options { get; } = new ReplicatorOptionsDictionary();

        internal Database OtherDB { get; }

        internal Uri RemoteUrl { get; }

        /// <summary>
        /// Constructs a configuration between two databases
        /// </summary>
        /// <param name="localDatabase">The local database for replication</param>
        /// <param name="targetDatabase">The target database to use as the endpoint</param>
        public ReplicatorConfiguration(Database localDatabase, Database targetDatabase)
        {
            Database = localDatabase ?? throw new ArgumentNullException(nameof(localDatabase));
            Target = OtherDB = targetDatabase ?? throw new ArgumentNullException(nameof(targetDatabase));
        }

        /// <summary>
        /// Constructs a configuration between a database and a remote URL
        /// </summary>
        /// <param name="localDatabase">The local database for replication</param>
        /// <param name="endpoint">The URL to replicate with</param>
        public ReplicatorConfiguration(Database localDatabase, Uri endpoint)
        {
            Database = localDatabase ?? throw new ArgumentNullException(nameof(localDatabase));
            Target = RemoteUrl = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        internal static ReplicatorConfiguration Clone(ReplicatorConfiguration source)
        {
            return (ReplicatorConfiguration) source.MemberwiseClone();
        }
    }
}