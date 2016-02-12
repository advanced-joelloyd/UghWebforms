using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace IRIS.Law.WebApp.App_Code
{
    public class XMLFunctions
    {
        // *********************************************************************
        // XPath element/attribute recogniser patterns.
        // Regular expressions using grouping constructs to parse and capture
        // embedded identifiers and values.
        // *********************************************************************
        const string identifierPattern = "[A-Za-z]{1}[A-Za-z0-9_\\-]{0,}";
        const string elementPattern = "^(" + identifierPattern + ")$";
        const string attributePattern = "^@(" + identifierPattern + ")$";
        const string elementWithAttributePattern =
            "^(" + identifierPattern + ")\\[@(" + identifierPattern + ")\\s{0,}=\\s{0,}['\"]{1}(.{1,})['\"]{1}\\]$";

        // *********************************************************************
        // Regular expression objects using the patterns defined above
        // *********************************************************************
        static Regex elementRegex = new Regex(elementPattern);
        static Regex attributeRegex = new Regex(attributePattern);
        static Regex elementWithAttributeRegex = new Regex(elementWithAttributePattern);

        /// ****************************************************************************************
        /// <summary>
        /// Get the Value from an XML Attribute for the supplied Document
        /// </summary>
        /// <param name="document">The document accepts XmlDocument value</param>
        /// <param name="XPath">The XPath accepts string value</param>
        /// <returns>string</returns>
        /// ****************************************************************************************
        public static string GetXMLAttributeValue(XmlDocument document, string XPath)
        {
            string value = string.Empty;
            //**********************************************************
            // xPath of the form "//Attributes/Attribute/@TaskId"
            //**********************************************************
            XmlAttribute attr = document.SelectSingleNode(XPath) as XmlAttribute;
            if (attr != null)
            {
                value = attr.Value;
            }
            return (string)value;
        }

        /// ****************************************************************************************
        /// <summary>
        /// Get the Value from an XML Attribute for the supplied Node
        /// </summary>
        /// <param name="node">The node accepts XmlNode value</param>
        /// <param name="XPath">The XPath accepts string value</param>
        /// <returns>string</returns>
        /// ****************************************************************************************
        public static string GetXMLAttributeValue(XmlNode node, string XPath)
        {
            string value = string.Empty;
            //**********************************************************
            // xPath of the form "//Attributes/Attribute/@TaskId"
            //**********************************************************
            XmlAttribute attr = node.SelectSingleNode(XPath) as XmlAttribute;
            if (attr != null)
            {
                value = attr.Value;
            }
            return (string)value;
        }

        /// ****************************************************************************************
        /// <summary>
        /// Get the InnerText from the supplied Document
        /// </summary>
        /// <param name="document">The document accepts XmlDocument value</param>
        /// <param name="XPath">The XPath accepts string value</param>
        /// <returns>string</returns>
        /// ****************************************************************************************
        public static string GetXMLAttributeInnerText(XmlDocument document, string XPath)
        {
            string value = string.Empty;
            //**********************************************************
            // xPath of the form "//Attributes/Attribute/Description"
            //**********************************************************
            XmlNode xNode;
            xNode = document.SelectSingleNode(XPath);
            if (xNode != null)
            {
                value = xNode.InnerText;
            }
            return (string)value;
        }

        /// ****************************************************************************************
        /// <summary>
        /// Get the InnerText from the supplied Document
        /// </summary>
        /// <param name="node">The node accepts XmlNode value</param>
        /// <param name="XPath">The XPath accepts string value</param>
        /// <returns>string</returns>
        /// ****************************************************************************************
        public static string GetXMLAttributeInnerText(XmlNode node, string XPath)
        {
            string value = string.Empty;
            //**********************************************************
            // xPath of the form "//Attributes/Attribute/Description"
            //**********************************************************
            XmlNode xNode;
            xNode = node.SelectSingleNode(XPath);
            if (xNode != null)
            {
                value = xNode.InnerText;
            }
            return (string)value;
        }

        /// ********************************************************************
        /// <summary>
        /// Update a DOM such that the value of a single node (determined by an 
        /// XPath expression) is set to the specified value.
        /// </summary>
        /// <param name="xmlNode">
        /// The DOM to be updated. Must be an XmlDocument or a node from an 
        /// XmlDocument.
        /// </param>
        /// <param name="xpath">
        /// A string containing a valid XPath expression for the DOM.
        /// Note that only a subset of the full XPath expression grammar is
        /// supported.
        /// </param>
        /// <param name="value">
        /// The value to be assigned to the node selected by the XPath 
        /// expression.
        /// </param>
        /// <returns>True if the update was successful.</returns>
        /// ********************************************************************
        public static bool SetXMLAttributeValue(XmlNode xmlNode, string xpath, string value)
        {
            bool result = false;
            XmlNode terminalNode = FindOrCreateNode(xmlNode, xpath);
            if (terminalNode != null)
            {
                XmlNodeType nodeType = terminalNode.NodeType;
                switch (nodeType)
                {
                    case XmlNodeType.Attribute:
                        terminalNode.Value = value;
                        result = true;
                        break;
                    case XmlNodeType.Element:
                        terminalNode.InnerText = value;
                        result = true;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// ********************************************************************
        /// <summary>
        /// Find or create the terminal node given by an XPath expression, with
        /// respect to a given starting node.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="xpath"></param>
        /// <returns>The located or created terminal node.</returns>
        /// ********************************************************************
        private static XmlNode FindOrCreateNode(XmlNode xmlNode, string xpath)
        {
            XmlNode terminalNode = null;

            // *****************************************************************
            // Check the validity of the XPath expression
            // *****************************************************************
            XPathNavigator navigator = xmlNode.CreateNavigator();
            XPathExpression expression = null;
            expression = navigator.Compile(xpath);
            Debug.Assert(expression != null);

            // *****************************************************************
            // Find the node selected by the XPath expression
            // *****************************************************************
            XmlNodeList nodes = xmlNode.SelectNodes(xpath);

            // *****************************************************************
            // The XPath expression should give rise to a single node/value
            // *****************************************************************
            if (nodes.Count > 1)
            {
                // *************************************************************
                // More than one matching node exists but we only have a single 
                // value to assign.
                // *************************************************************
                throw new Exception(" FindOrCreateNode: Invalid XPath expression for DOM update '" + xpath + "'");
            }
            else if (1 == nodes.Count)
            {
                // *************************************************************
                // A single matching node exists
                // *************************************************************
                terminalNode = nodes[0];
            }
            else
            {
                // *************************************************************
                // A matching node does not exist. 
                // Find the deepest existing parent node.
                // *************************************************************
                int index = FindExistingParentNode(xmlNode, xpath);

                // *************************************************************
                // Create the missing element/attribute nodes under the existing 
                // parent node.
                // *************************************************************
                terminalNode = CreateTerminalNode(xmlNode, xpath, index);
            }

            return terminalNode;
        }

        /// ********************************************************************
        /// <summary>
        /// This function will search backwards along an XPath expression until
        /// a matching parent node is found in a DOM.
        /// </summary>
        /// <param name="xmlNode">
        /// The DOM that is to be searched for a matching node.
        /// </param>
        /// <param name="xpath">
        /// The XPath expression string that will be used to find a suitable 
        /// node.
        /// </param>
        /// <returns>
        /// An index into the XPath expression string, indicating where the
        /// first non-existent node occurs.
        /// </returns>
        /// ********************************************************************
        private static int FindExistingParentNode(XmlNode xmlNode, string xpath)
        {
            // *****************************************************************
            // Clone the original XPath expression as we will need to modify it
            // *****************************************************************
            string newXPath = xpath.Clone() as string;
            int index = newXPath.Length - 1;

            // *****************************************************************
            // Keep searching until a matching parent node is found
            // *****************************************************************
            XmlNodeList nodes = xmlNode.SelectNodes(newXPath);
            while (nodes.Count < 1)
            {
                index = newXPath.LastIndexOf('/', index);
                if (index > 1)
                {
                    // *********************************************************
                    // Truncate the XPath expression by removing the final 
                    // sub-expression.
                    // *********************************************************
                    newXPath = newXPath.Substring(0, index);
                    nodes = xmlNode.SelectNodes(newXPath);
                    index = newXPath.Length - 1;
                }
                else
                {
                    // *********************************************************
                    // Reached the beginning of the XPath expression. Bail out.
                    // *********************************************************
                    break;
                }
            }

            // *****************************************************************
            // Return the index of the character following the last '/'
            // *****************************************************************
            return index + 1;
        }

        /// ********************************************************************
        /// <summary>
        /// Advances through an XPath expression from a given starting point,
        /// creating element and attribute nodes in the DOM as necessary to 
        /// ensure that the full XPath expression matches a single node in the
        /// DOM.
        /// Missing node types are determined by applying a regular expression
        /// based recogniser to each XPath sub-expression in turn.
        /// </summary>
        /// 
        /// <param name="xmlNode"></param>
        /// The DOM in which the missing nodes are to be updated. Must be an 
        /// XmlDocument or a node from an XmlDocument.
        /// <param name="xpath"></param>
        /// The XPath expression that implies those nodes that must exist or
        /// need to be created.
        /// <param name="startIndex">
        /// An index into the XPath expression string indicating the first sub-
        /// expression for which a node must be created.
        /// </param>
        /// <returns>
        /// An XmlNode that is the terminal node implied by the full XPath 
        /// expression.
        /// </returns>
        /// ********************************************************************
        private static XmlNode CreateTerminalNode(XmlNode xmlNode, string xpath, int startIndex)
        {
            // ****************************************************************
            // Determine the owning document for the XML node
            // ****************************************************************
            XmlDocument xmlDoc = null;
            if (xmlNode is XmlDocument)
            {
                xmlDoc = xmlNode as XmlDocument;
            }
            else
            {
                xmlDoc = xmlNode.OwnerDocument;
            }

            if (null == xmlDoc)
            {
                throw new Exception(" CreateTerminalNode: XmlNode does not appear to be associated with an XmlDocument");
            }

            // *****************************************************************
            // Find the existing node using a subset of the XPath expression
            // *****************************************************************
            string subPath = null;
            XmlNode newNode = null;

            if (startIndex > 0)
            {
                // *************************************************************
                // Existing node was previously found
                // *************************************************************
                subPath = xpath.Substring(0, startIndex);
                XmlNodeList nodes = xmlNode.SelectNodes(subPath);
                Debug.Assert(1 == nodes.Count);
                newNode = nodes[0];
            }
            else
            {
                // *************************************************************
                // No exisiting node, create a new node relative to the DOM node
                // *************************************************************
                subPath = "";
                if (xmlNode is XmlDocument)
                {
                    newNode = ((XmlDocument)xmlNode).DocumentElement;
                }
                else
                {
                    newNode = xmlNode;
                }
            }

            // *****************************************************************
            // Advance along the xpath expression, creating matching nodes
            //
            // This requires something approaching a full XPath parser.
            // Potentially, we need to deal with:-
            //   * Wildcards
            //   * Relative location paths
            //   * Namespaces
            //   * Predicates
            //   * Numerical expressions
            //   * Boolean expressions
            //   * XPath functions (e.g. text())
            //
            // For now, implement a simple XPath sub-expression parser using a
            // regular expression based recogniser.
            // *****************************************************************
            while (startIndex < xpath.Length)
            {
                XmlNode parentNode = newNode;

                // *************************************************************
                // Extract the next XPath sub-expression
                // *************************************************************
                int start = startIndex;
                if (startIndex > 0)
                {
                    start = xpath.IndexOf('/', startIndex) + 1;
                }
                int end = xpath.IndexOf('/', start);
                if (end < start)
                {
                    end = xpath.Length;
                }
                string subExpression = xpath.Substring(start, (end - start));

                // *************************************************************
                // Determine the types of nodes that must be created
                // *************************************************************
                Match match = null;

                if (elementRegex.IsMatch(subExpression))
                {
                    // *********************************************************
                    // New node is an element
                    // *********************************************************
                    match = elementRegex.Match(subExpression);
                    string elementName = match.Groups[1].Value;
                    newNode = xmlDoc.CreateElement(elementName);
                    parentNode.AppendChild(newNode);
                }
                else if (attributeRegex.IsMatch(subExpression))
                {
                    // *********************************************************
                    // New node is an attribute
                    // *********************************************************
                    match = attributeRegex.Match(subExpression);
                    string attributeName = match.Groups[1].Value;
                    newNode =
                        xmlDoc.CreateAttribute(attributeName) as XmlAttribute;
                    parentNode.Attributes.Append(newNode as XmlAttribute);
                }
                else if (elementWithAttributeRegex.IsMatch(subExpression))
                {
                    // *********************************************************
                    // New node is an element with a specific attribute value
                    // *********************************************************
                    match = elementWithAttributeRegex.Match(subExpression);
                    int n = match.Groups.Count;
                    string elementName = match.Groups[1].Value;
                    string attributeName = match.Groups[2].Value;
                    string attributeValue = match.Groups[3].Value;
                    newNode = xmlDoc.CreateElement(elementName);
                    XmlAttribute newAttribute =
                        xmlDoc.CreateAttribute(attributeName)
                        as XmlAttribute;
                    newAttribute.Value = attributeValue;
                    newNode.Attributes.Append(newAttribute);
                    parentNode.AppendChild(newNode);
                }
                else
                {
                    // *********************************************************
                    // Unsupported sub-expression
                    // *********************************************************
                    throw new Exception(" CreateTerminalNode: Cannot create node for XPath expression '" + subExpression + "' (unsupported sub-expression type)");
                }

                // *************************************************************
                // Advance to the next sub-expression
                // *************************************************************
                startIndex = end;
            }

            return newNode;
        }

    }
}