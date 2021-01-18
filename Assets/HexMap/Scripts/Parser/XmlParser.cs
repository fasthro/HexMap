/*
 * @Author: fasthro
 * @Date: 2021-01-06 17:17:08
 * @Description: 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Mono.Xml;
using UnityEngine;

namespace HexMap
{
    public abstract class XmlParser
    {
        protected string _xmlPath;
        protected SecurityElement _rootElement;

        public bool isLoaded { get; private set; }
        public bool isLoading { get; private set; }

        public bool isParsed { get; private set; }
        public bool isParsinging { get; private set; }

        public XmlParser(string xmlPath)
        {
            _xmlPath = xmlPath;
        }

        public void LoadXml(bool useThread = true)
        {
            if (isLoading) return;
            isLoaded = false;
            isLoading = true;
            if (useThread)
            {
                ThreadUtils.Run<string>(Load, _xmlPath, onLoaded);
            }
            else
            {
                Load(_xmlPath);
                onLoaded();
            }
        }

        private void onLoaded()
        {
            isLoading = false;
            isLoaded = true;
        }

        private void Load(string path)
        {
            SecurityParser sp = new SecurityParser();
            var data = File.ReadAllText(path);
            sp.LoadXml(data.ToString());
            _rootElement = sp.ToXml();
        }

        public void ParseXml(bool useThread = true)
        {
            if (isParsinging) return;
            isParsed = false;
            isParsinging = true;
            if (useThread)
            {
                ThreadUtils.Run(OnParseXml, OnParsed);
            }
            else
            {
                OnParseXml();
                OnParsed();
            }
        }

        protected abstract void OnParseXml();

        private void OnParsed()
        {
            isParsed = true;
            isParsinging = false;
        }
    }
}