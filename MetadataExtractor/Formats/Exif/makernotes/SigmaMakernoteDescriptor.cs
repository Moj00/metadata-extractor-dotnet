/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="SigmaMakernoteDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class SigmaMakernoteDescriptor : TagDescriptor<SigmaMakernoteDirectory>
    {
        public SigmaMakernoteDescriptor([NotNull] SigmaMakernoteDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case SigmaMakernoteDirectory.TagExposureMode:
                {
                    return GetExposureModeDescription();
                }

                case SigmaMakernoteDirectory.TagMeteringMode:
                {
                    return GetMeteringModeDescription();
                }
            }
            return base.GetDescription(tagType);
        }

        [CanBeNull]
        private string GetMeteringModeDescription()
        {
            var value = Directory.GetString(SigmaMakernoteDirectory.TagMeteringMode);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            switch (value[0])
            {
                case '8':
                {
                    return "Multi Segment";
                }

                case 'A':
                {
                    return "Average";
                }

                case 'C':
                {
                    return "Center Weighted Average";
                }

                default:
                {
                    return value;
                }
            }
        }

        [CanBeNull]
        private string GetExposureModeDescription()
        {
            var value = Directory.GetString(SigmaMakernoteDirectory.TagExposureMode);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            switch (value[0])
            {
                case 'A':
                {
                    return "Aperture Priority AE";
                }

                case 'M':
                {
                    return "Manual";
                }

                case 'P':
                {
                    return "Program AE";
                }

                case 'S':
                {
                    return "Shutter Speed Priority AE";
                }

                default:
                {
                    return value;
                }
            }
        }
    }
}
