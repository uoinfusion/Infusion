using FluentAssertions;
using Infusion.Desktop.Profiles;
using Infusion.LegacyApi.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Infusion.Desktop.Tests.Profiles
{
    [TestClass]
    public class ProfileConfigSerializationTests
    {
        [TestMethod]
        public void Can_roundtrip_int_property()
            => DoRoundtrip(123).Should().Be(123);

        [TestMethod]
        public void Can_roundtrip_string_property()
            => DoRoundtrip("some string").Should().Be("some string");

        [TestMethod]
        public void Can_roundtrip_object_with_array_of_int_property()
        {
            var result = DoRoundtrip(new
            {
                P1 = new int[] { 1, 2, 3 }
            });

            result.P1.Should().BeEquivalentTo(1, 2, 3);
        }

        [TestMethod]
        public void Can_roundtrip_object_with_array_of_other_object()
        {
            var result = DoRoundtrip(new
            {
                P1 = new[] { new { X1 = 123 }, new { X1 = 234 } }
            });

            result.P1[0].X1.Should().Be(123);
            result.P1[1].X1.Should().Be(234);
        }

        [TestMethod]
        public void Can_roundrip_ObjectId_property_serializing_to_uint()
        {
            var result = DoRoundtrip(new ObjectId(0x40001234), out string json);
            result.Should().Be(new ObjectId(0x40001234));
            json.Should().NotContain("\"Value\":");
        }

        [TestMethod]
        public void Can_roundrip_object_with_ObjectId_property_serializing_to_uint()
        {
            var result = DoRoundtrip(new { Id = new ObjectId(0x40001234) }, out string json);
            result.Id.Should().Be(new ObjectId(0x40001234));
            json.Should().NotContain("\"Value\":");
        }

        [TestMethod]
        public void Can_roundtrip_object_with_enumerable_property()
        {
            var result = DoRoundtrip(new ObjectWithEnumerableProperty
            {
                P1 = new[] { 1, 2, 3 }
            });

            result.P1.Should().BeEquivalentTo(1, 2, 3);
        }

        private class ObjectWithEnumerableProperty
        {
            private List<int> f1 = new List<int>();

            public IEnumerable<int> P1
            {
                get => f1;
                set => f1 = new List<int>(value);
            }
        }

        private T DoRoundtrip<T>(T value, out string json)
        {
            var propertyName = "test";
            var profile = new LaunchProfile();
            profile.Options[propertyName] = value;

            json = ProfileRepository.SerializeProfile(profile);
            var roundtrippedProfile = ProfileRepository.DeserializeProfile(json);

            var profileConfigRepository = new ProfileConfigRepository(roundtrippedProfile, new NullConsole());

            return profileConfigRepository.Get<T>(propertyName);
        }

        private T DoRoundtrip<T>(T value)
            => DoRoundtrip(value, out string json);
    }
}
