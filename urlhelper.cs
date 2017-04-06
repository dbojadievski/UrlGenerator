using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using static System.Diagnostics.Debug;

namespace Api.Helpers
{
    internal class StringValue : System.Attribute
    {
        private readonly String value;

        public StringValue ( String value )
        {
            this.value                  = value;
        }

        public String Value
        {
            get
            {
                return this.value;
            }
        }


    }
    public enum NetworkProtocol
    {
        UNUSED = 0,
        [StringValue("http")]
        Http,
        [StringValue("https")]
        Https,
        [StringValue("ws")]
        WebSockets
    }

    public static class StringEnum
    {
        public static string GetStringValue ( Enum value )
        {
            string output = null;
            Type type = value.GetType();

            //Check first in our cached results...

            //Look for our 'StringValueAttribute' 

            //in the field's custom attributes

            FieldInfo fi        = type.GetField(value.ToString());
            StringValue[] attrs =
           fi.GetCustomAttributes(typeof(StringValue),
                                   false) as StringValue[];
            if ( attrs.Length > 0 )
            {
                output          = attrs [ 0 ].Value;
            }

            return output;
        }
    }
    public class Url
    {
        private String root;

        private List<String> pathSegments;
        private Dictionary<String, String> urlParams;

        private NetworkProtocol protocol;

        public static implicit operator Url ( String s )
        {
            Url url                     = new Url ( s );
            return url;
        }

        public static implicit operator String ( Url u )
        {
            return u.ToString ( );
        }

        public Url AppendSection ( String section )
        {
            this.pathSegments.Add ( section );
            return this;
        }

        public Url AppendSection ( Int32 section )
        {
            String asString                 = Convert.ToString ( section );
            this.pathSegments.Add ( asString );

            return this;
        }

        public Url AppendParameter ( String name, String value )
        {
            this.urlParams.Add ( name, value );
            return this;
        }

        public Url ( String urlRoot )
        {
            Int32 urlProtocolIdx            = urlRoot.IndexOf ( "://" );

            NetworkProtocol protocol        = NetworkProtocol.Http;
            if ( urlProtocolIdx != -1 )
            {
                protocol                    = NetworkProtocol.Http;
                String protocolString       = urlRoot.Substring ( 0, urlProtocolIdx ).ToLower ( );
                switch ( protocolString )
                {
                    case "http":
                        protocol        = NetworkProtocol.Http;
                        break;
                    case "https":
                        protocol        = NetworkProtocol.Https;
                        break;
                    case "ws":
                        protocol        = NetworkProtocol.WebSockets;
                        break;
                    default:
                        Assert ( false );
                        break;
                }

                urlRoot                 = urlRoot.Substring ( urlProtocolIdx + 3 );
            }

            this.root                   = urlRoot;

            this.urlParams              = new Dictionary<string, string> ( );
            this.pathSegments           = new List<string> ( );

            this.protocol               = protocol;
        }

        public String GeneratedUrl
        {
            get
            {
                return this.ToString ( );
            }
        }

        public override String ToString ( )
        {
            StringBuilder builder                       = new StringBuilder ( );

            String protoString                          = String.Format ( "{0}://", StringEnum.GetStringValue ( this.protocol ) );
            builder.Append ( protoString );

            builder.Append ( this.root );

            foreach ( String segment in this.pathSegments )
                builder.Append ( String.Format ( "/{0}", segment ) );

            if ( this.urlParams.Count > 0 )
            {
                builder.Append ( "?" );
                Int32 stopIdx                           = ( this.urlParams.Count - 1 );
                for ( Int32 currParamIdx = 0; currParamIdx <= stopIdx; currParamIdx++ )
                {
                    KeyValuePair<String, String> param  = this.urlParams.ElementAt ( currParamIdx );
                    String paramString                  = String.Format ("{0}={1}", param.Key, param.Value );
                    builder.Append ( paramString );

                    if ( currParamIdx < stopIdx )
                        builder.Append ( "&" );
                }
            }

            return builder.ToString ( );
        }
    }
}
