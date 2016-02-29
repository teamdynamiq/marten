﻿using System;
using System.Linq;
using Marten.Services;
using Shouldly;
using Xunit;

namespace Marten.Testing
{
    public class detecting_insert_or_update_Tests : DocumentSessionFixture<NulloIdentityMap>
    {
        [Fact]
        public void numeric_zero_id_is_insert()
        {
            var doc = new NumericDoc {Id = 0};

            theSession.Store(doc);

            theSession.PendingChanges.InsertsFor<NumericDoc>().Single().ShouldBeTheSameAs(doc);
            theSession.PendingChanges.Inserts().Single().ShouldBeTheSameAs(doc);

            theSession.PendingChanges.Updates().Any().ShouldBeFalse();

            doc.Id.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void numeric_not_zero_is_update()
        {
            var doc = new NumericDoc { Id = 1 };

            theSession.Store(doc);

            theSession.PendingChanges.UpdatesFor<NumericDoc>().Single().ShouldBeTheSameAs(doc);
            theSession.PendingChanges.Updates().Single().ShouldBeTheSameAs(doc);

            theSession.PendingChanges.Inserts().Any().ShouldBeFalse();

            doc.Id.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void guid_empty_should_be_insert()
        {
            var doc = new GuidDoc { Id = default(Guid) };

            theSession.Store(doc);

            theSession.PendingChanges.InsertsFor<GuidDoc>().Single().ShouldBeTheSameAs(doc);
            theSession.PendingChanges.Inserts().Single().ShouldBeTheSameAs(doc);

            theSession.PendingChanges.Updates().Any().ShouldBeFalse();

            doc.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void guid_not_empty_is_update()
        {
            var theId = Guid.NewGuid();
            var doc = new GuidDoc { Id = theId };

            theSession.Store(doc);

            theSession.PendingChanges.UpdatesFor<GuidDoc>().Single().ShouldBeTheSameAs(doc);
            theSession.PendingChanges.Updates().Single().ShouldBeTheSameAs(doc);

            theSession.PendingChanges.Inserts().Any().ShouldBeFalse();

            // 
            doc.Id.ShouldBe(theId);
        }
    }

    public class NumericDoc
    {
        public int Id;
    }

    public class GuidDoc
    {
        public Guid Id;
    }
}