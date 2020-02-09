﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslValidation = global::Microsoft.VisualStudio.Modeling.Validation;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;
namespace Sawczyn.EFDesigner.EFModel
{
	
	public abstract partial class EFModelSerializationHelperBase
	{
		protected virtual bool IsValid(string diagramsFileName)
		{
			var l_fileInfo = new global::System.IO.FileInfo(diagramsFileName);
	        return (l_fileInfo.Exists && (l_fileInfo.Length > 10));
		}
		
		private DslModeling::DomainClassXmlSerializer GetSerializer(DslModeling::Store store, string localName)
	    {
			var directory = this.GetDirectory(store);
			switch (localName)
	        {
	            case "wpfdiagram":
	                //return this.GetDirectory(store).GetSerializer(WPFDiagram.DomainClassId);
					return null;
	            default:
	                return this.GetDirectory(store).GetSerializer(EFModelDiagram.DomainClassId);
	        }
		}
		
		private DslModeling::DomainClassXmlSerializer GetSerializer(DslDiagrams::Diagram diagram)
	    {
			var directory = this.GetDirectory(diagram.Store);
			var diagramSerializer = directory.GetSerializer(diagram.GetDomainClass().Id) ?? directory.GetSerializer(EFModelDiagram.DomainClassId);
			return diagramSerializer;
		}
		
		protected virtual void WriteRootElement(DslModeling::SerializationContext serializationContext, DslModeling::DomainClassXmlSerializer domainClassXmlSerializer, DslModeling::ModelElement rootElement, global::System.Xml.XmlWriter writer)
		{
			#region Check Parameters
			global::System.Diagnostics.Debug.Assert(serializationContext != null);
			if (serializationContext == null)
				throw new global::System.ArgumentNullException("serializationContext");
			global::System.Diagnostics.Debug.Assert(domainClassXmlSerializer != null);
			if (domainClassXmlSerializer == null)
				throw new global::System.ArgumentNullException("domainClassXmlSerializer");
			global::System.Diagnostics.Debug.Assert(rootElement != null);
			if (rootElement == null)
				throw new global::System.ArgumentNullException("rootElement");
			global::System.Diagnostics.Debug.Assert(writer != null);
			if (writer == null)
				throw new global::System.ArgumentNullException("writer");
			#endregion
	
			// Set up root element settings
			DslModeling::RootElementSettings rootElementSettings = new DslModeling::RootElementSettings();
			if (!(rootElement is DslDiagrams::Diagram))
			{
				// Only model has schema, diagram has no schema.
				rootElementSettings.SchemaTargetNamespace = "http://schemas.microsoft.com/dsltools/Language4";
			}
			rootElementSettings.Version = new global::System.Version("1.0.0.0");
	
			// Carry out the normal serialization.
			domainClassXmlSerializer.Write(serializationContext, rootElement, writer, rootElementSettings);
		}
	
		private global::System.IO.MemoryStream InternalSaveDiagram(DslModeling::SerializationResult serializationResult, DslDiagrams::Diagram diagram, string diagramFileName, global::System.Text.Encoding encoding, bool writeOptionalPropertiesWithDefaultValue)
		{
			#region Check Parameters
			global::System.Diagnostics.Debug.Assert(serializationResult != null);
			global::System.Diagnostics.Debug.Assert(diagram != null);
			global::System.Diagnostics.Debug.Assert(!serializationResult.Failed);
			#endregion
		
			global::System.IO.MemoryStream newFileContent = new global::System.IO.MemoryStream();
			var directory = this.GetDirectory(diagram.Store);
			DslModeling::SerializationContext serializationContext = new DslModeling::SerializationContext(directory, diagramFileName, serializationResult);
			this.InitializeSerializationContext(diagram.Partition, serializationContext, false);
			// MonikerResolver shouldn't be required in Save operation, so not calling SetupMonikerResolver() here.
			serializationContext.WriteOptionalPropertiesWithDefaultValue = writeOptionalPropertiesWithDefaultValue;
			global::System.Xml.XmlWriterSettings settings = EFModelSerializationHelper.Instance.CreateXmlWriterSettings(serializationContext, true, encoding);
			var diagramSerializer = GetSerializer(diagram);	
			
			using (global::System.Xml.XmlWriter writer = global::System.Xml.XmlWriter.Create(newFileContent, settings))
			{
				//this.WriteRootElement(serializationContext, diagram, writer);
				// Carry out the normal serialization.
				this.WriteRootElement(serializationContext, diagramSerializer, diagram, writer);
			}
	
			return newFileContent;
		}
	
		protected virtual void ReadRootElement(DslModeling::SerializationContext serializationContext, DslModeling::DomainClassXmlSerializer domainClassXmlSerializer, DslModeling::ModelElement rootElement, global::System.Xml.XmlReader reader, DslModeling::ISchemaResolver schemaResolver)
		{
			#region Check Parameters
			global::System.Diagnostics.Debug.Assert(serializationContext != null);
			if (serializationContext == null)
				throw new global::System.ArgumentNullException("serializationContext");
			global::System.Diagnostics.Debug.Assert(domainClassXmlSerializer != null);
			if (domainClassXmlSerializer == null)
				throw new global::System.ArgumentNullException("domainClassXmlSerializer");
			global::System.Diagnostics.Debug.Assert(rootElement != null);
			if (rootElement == null)
				throw new global::System.ArgumentNullException("rootElement");
			global::System.Diagnostics.Debug.Assert(reader != null);
			if (reader == null)
				throw new global::System.ArgumentNullException("reader");
			#endregion
	
			// Version check.
			this.CheckVersion(serializationContext, reader);
	
			if (!serializationContext.Result.Failed)
			{	
				// Use a validating reader if possible
				using (reader = TryCreateValidatingReader(schemaResolver, reader, serializationContext))
				{
					domainClassXmlSerializer.Read(serializationContext, rootElement, reader);
				}
			}
	
		}
	
		// private void OnPostLoadModelAndDiagram(DslModeling::SerializationResult serializationResult, DslModeling::Partition modelPartition, string modelFileName, DslModeling::Partition diagramPartition, string diagramFileName, ModelRoot modelRoot, EFModelDiagram diagram)
		// private DslDiagrams::Diagram LoadDiagram(DslModeling::SerializationResult serializationResult, DslModeling::ModelElement modelRoot, DslModeling::Partition diagramPartition, global::System.IO.Stream diagramStream, DslModeling::ISchemaResolver schemaResolver, DslValidation::ValidationController validationController, DslModeling::ISerializerLocator serializerLocator)
		private DslDiagrams::Diagram LoadDiagram(DslModeling::SerializationResult serializationResult, DslModeling::Partition modelPartition, string modelFileName, string diagramFileName, DslModeling::ModelElement modelRoot, DslModeling::Partition diagramPartition, global::System.IO.Stream diagramStream, DslModeling::ISchemaResolver schemaResolver, DslValidation::ValidationController validationController, DslModeling::ISerializerLocator serializerLocator)
		{
			#region Check Parameters
			if (serializationResult == null)
				throw new global::System.ArgumentNullException("serializationResult");
			if (modelRoot == null)		
				throw new global::System.ArgumentNullException("modelRoot");
			if (diagramPartition == null)
				throw new global::System.ArgumentNullException("diagramPartition");
			#endregion
			
			DslDiagrams::Diagram diagram = null;
			var diagramName = string.Empty;
		
			// Ensure there is an outer transaction spanning both model and diagram load, so moniker resolution works properly.
			if (!diagramPartition.Store.TransactionActive)
			{
				throw new global::System.InvalidOperationException(EFModelDomainModel.SingletonResourceManager.GetString("MissingTransaction"));
			}
			
			if (diagramStream == null || diagramStream == global::System.IO.Stream.Null || !diagramStream.CanRead || diagramStream.Length < 6)
	        {
	            // missing diagram file indicates we should create a new diagram.
	            diagram = this.CreateDiagramHelper(diagramPartition, modelRoot);
	        }
			else
			{
				var directory = this.GetDirectory(diagramPartition.Store);
				var localName = string.Empty;
				var localSettings = EFModelSerializationHelper.Instance.CreateXmlReaderSettings(null, false);
				try
				{
					using (var reader = global::System.Xml.XmlReader.Create(diagramStream, localSettings))
			        {
			            reader.MoveToContent();
			            localName = reader.LocalName;
						global::System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(reader.GetAttribute("name")), "One of diagram streams is not well-formed");
						diagramName = reader.GetAttribute("name");
					}
					diagramStream.Seek(0, global::System.IO.SeekOrigin.Begin);
				}
				catch (global::System.Xml.XmlException xEx)
				{
					DslModeling::SerializationUtilities.AddMessage(
						new DslModeling::SerializationContext(directory),
						DslModeling::SerializationMessageKind.Error,
						xEx
					);
				}
				
				var diagramSerializer = directory.GetSerializer(EFModelDiagram.DomainClassId) ?? this.GetSerializer(diagramPartition.Store, localName);		
				global::System.Diagnostics.Debug.Assert(diagramSerializer != null, "Cannot find serializer for " + diagramName);
				
				if (diagramSerializer != null)
				{
					DslModeling::SerializationContext serializationContext = new DslModeling::SerializationContext(directory, diagramName, serializationResult);
					this.InitializeSerializationContext(diagramPartition, serializationContext, true);
					DslModeling::TransactionContext transactionContext = new DslModeling::TransactionContext();
					transactionContext.Add(DslModeling::SerializationContext.TransactionContextKey, serializationContext);
					
		
						using (DslModeling::Transaction postT = diagramPartition.Store.TransactionManager.BeginTransaction("PostLoad Model and Diagram", true, transactionContext))
						{
		
						using (DslModeling::Transaction t = diagramPartition.Store.TransactionManager.BeginTransaction("LoadDiagram", true, transactionContext))
						{
							// Ensure there is some content in the file. Blank (or almost blank, to account for encoding header bytes, etc.)
							// files will cause a new diagram to be created and returned 
							if (diagramStream.Length > 5)
							{
								global::System.Xml.XmlReaderSettings settings = EFModelSerializationHelper.Instance.CreateXmlReaderSettings(serializationContext, false);
								try
								{
									using (global::System.Xml.XmlReader reader = global::System.Xml.XmlReader.Create(diagramStream, settings))
									{
										reader.MoveToContent();
										diagram = diagramSerializer.TryCreateInstance(serializationContext, reader, diagramPartition) as DslDiagrams::Diagram;
										if (diagram != null)
										{
											this.ReadRootElement(serializationContext, diagramSerializer, diagram, reader, schemaResolver);
										}
									}
								}
								catch (global::System.Xml.XmlException xEx)
								{
									DslModeling::SerializationUtilities.AddMessage(
										serializationContext,
										DslModeling::SerializationMessageKind.Error,
										xEx
									);
								}
								if (serializationResult.Failed)
								{	
									// Serialization error encountered, rollback the transaction.
									diagram = null;
									t.Rollback();
								}
							}
						
							if(diagram == null && !serializationResult.Failed)
							{
								// Create diagram if it doesn't exist
								diagram = this.CreateDiagramHelper(diagramPartition, modelRoot);
							}
							
							if (t.IsActive)
								t.Commit();
						} // End inner Tx
		
	
							// Fire PostLoad customization code whether Load succeeded or not
							// Provide a method in a partial class with the following signature:
							
							///// <summary>
							///// Customize Model and Diagram Loading.
							///// </summary>
							///// <param name="serializationResult">Stores serialization result from the load operation.</param>
							///// <param name="modelPartition">Partition in which the new DslLibrary instance will be created.</param>
							///// <param name="modelFileName">Name of the file from which the DslLibrary instance will be deserialized.</param>
							///// <param name="diagramPartition">Partition in which the new DslDesignerDiagram instance will be created.</param>
							///// <param name="diagramFileName">Name of the file from which the DslDesignerDiagram instance will be deserialized.</param>
							///// <param name="modelRoot">The root of the file that was loaded.</param>
							///// <param name="diagram">The diagram matching the modelRoot.</param>
							// private void OnPostLoadModelAndDiagram(DslModeling::SerializationResult serializationResult, DslModeling::Partition modelPartition, string modelFileName, DslModeling::Partition diagramPartition, string diagramFileName, ModelRoot modelRoot, EFModelDiagram diagram)
	
							this.OnPostLoadModelAndDiagram(serializationResult, modelPartition, modelFileName, diagramPartition, diagramFileName, (ModelRoot)modelRoot, (EFModelDiagram)diagram);
							if (serializationResult.Failed)
							{	// Serialization error encountered, rollback the middle transaction.
									modelRoot = null;
									postT.Rollback();
							}
							if (postT.IsActive)
								postT.Commit();
						} // End MiddleTx					
					// Do load-time validation if a ValidationController is provided.
					if (!serializationResult.Failed && validationController != null)
					{
						using (new SerializationValidationObserver(serializationResult, validationController))
						{
							validationController.Validate(diagramPartition, DslValidation::ValidationCategories.Load);
						}
					}
				}
			}
			
			if (diagram != null)
			{
				if (!serializationResult.Failed)
				{	// Succeeded.
					diagram.ModelElement = diagram.ModelElement ?? modelRoot;
					diagram.PostDeserialization(true);
					this.CheckForOrphanedShapes(diagram, serializationResult);
				}
				else
				{	// Failed.
					diagram.PostDeserialization(false);
				}
			}
			
			return diagram;
		}
	
		public virtual ModelRoot LoadModelAndDiagrams(DslModeling::Store store, string modelFileName, string diagramsFileName, DslModeling::ISchemaResolver schemaResolver, DslValidation::ValidationController validationController, DslModeling::ISerializerLocator serializerLocator)
		{
			return this.LoadModelAndDiagrams(new DslModeling::SerializationResult(), store, modelFileName, diagramsFileName, schemaResolver, validationController, serializerLocator);
		}
		
		public virtual ModelRoot LoadModelAndDiagrams(DslModeling::SerializationResult serializationResult, DslModeling::Store store, string modelFileName, string diagramsFileName, DslModeling::ISchemaResolver schemaResolver, DslValidation::ValidationController validationController, DslModeling::ISerializerLocator serializerLocator)
		{
			#region Check Parameters
			if (store == null)
				throw new global::System.ArgumentNullException("store");
			#endregion
			
			DslModeling::Partition diagramsPartition = new DslModeling::Partition(store);
			return this.LoadModelAndDiagrams(serializationResult, store.DefaultPartition, modelFileName, diagramsPartition, diagramsFileName, schemaResolver, validationController, serializerLocator);
		}
		
		public virtual ModelRoot LoadModelAndDiagrams(DslModeling::SerializationResult serializationResult, DslModeling::Partition modelPartition, string modelFileName, DslModeling::Partition diagramsPartition, string diagramsFileName, DslModeling::ISchemaResolver schemaResolver, DslValidation::ValidationController validationController, DslModeling::ISerializerLocator serializerLocator)
		{
			#region Check Parameters
			if (serializationResult == null)
				throw new global::System.ArgumentNullException("serializationResult");
			if (modelPartition == null)		
				throw new global::System.ArgumentNullException("modelPartition");
			if (diagramsPartition == null)
				throw new global::System.ArgumentNullException("diagramsPartition");
			if (string.IsNullOrEmpty(diagramsFileName))
				throw new global::System.ArgumentNullException("diagramsFileName");
			#endregion
	
			ModelRoot modelRoot;
	
			// Ensure there is an outer transaction spanning both model and diagram load, so moniker resolution works properly.
			if (!diagramsPartition.Store.TransactionActive)
			{
				throw new global::System.InvalidOperationException(EFModelDomainModel.SingletonResourceManager.GetString("MissingTransaction"));
			}
	
			modelRoot = this.LoadModel(serializationResult, modelPartition, modelFileName, schemaResolver, validationController, serializerLocator);
	
			if (serializationResult.Failed)
			{
				// don't try to deserialize diagram data if model load failed.
				return modelRoot;
			}
			
			if (IsValid(diagramsFileName))
	        {
				using (var pkgOutputDoc = global::System.IO.Packaging.Package.Open(diagramsFileName, global::System.IO.FileMode.Open, global::System.IO.FileAccess.Read))
	            {
	                foreach (var packagePart in pkgOutputDoc.GetParts())
	                {
	                    this.LoadDiagram(serializationResult, modelPartition, modelFileName, diagramsFileName, modelRoot, diagramsPartition, packagePart.GetStream(global::System.IO.FileMode.Open, global::System.IO.FileAccess.Read), schemaResolver, validationController, serializerLocator);
	                }
	            }
			}
			else
			{
				// missing diagram file indicates we should create a new diagram.
				this.LoadDiagram(serializationResult, modelPartition, modelFileName, diagramsFileName, modelRoot, diagramsPartition, global::System.IO.Stream.Null, schemaResolver, validationController, serializerLocator);
			}
		
			return modelRoot;
		}
		
		public virtual void SaveModelAndDiagrams(DslModeling::SerializationResult serializationResult, ModelRoot modelRoot, string modelFileName, DslDiagrams::Diagram[] diagrams, string diagramsFileName)
		{
			this.SaveModelAndDiagrams(serializationResult, modelRoot, modelFileName, diagrams, diagramsFileName, global::System.Text.Encoding.UTF8, false);
		}
		
		public virtual void SaveModelAndDiagrams(DslModeling::SerializationResult serializationResult, ModelRoot modelRoot, string modelFileName, DslDiagrams::Diagram[] diagrams, string diagramsFileName, bool writeOptionalPropertiesWithDefaultValue)
		{
			this.SaveModelAndDiagrams(serializationResult, modelRoot, modelFileName, diagrams, diagramsFileName, global::System.Text.Encoding.UTF8, writeOptionalPropertiesWithDefaultValue);
		}
		
		public virtual void SaveModelAndDiagrams(DslModeling::SerializationResult serializationResult, ModelRoot modelRoot, string modelFileName, DslDiagrams::Diagram[] diagrams, string diagramsFileName, global::System.Text.Encoding encoding, bool writeOptionalPropertiesWithDefaultValue)
		{
			#region Check Parameters
			if (serializationResult == null)
				throw new global::System.ArgumentNullException("serializationResult");
			if (string.IsNullOrEmpty(modelFileName))
				throw new global::System.ArgumentNullException("modelFileName");
			if (diagrams == null)
				throw new global::System.ArgumentNullException("diagrams");
			if (string.IsNullOrEmpty(diagramsFileName))
				throw new global::System.ArgumentNullException("diagramsFileName");
			#endregion
	
			if (serializationResult.Failed)
				return;
				
			// Save the model file first
			var modelFileContent = this.InternalSaveModel(serializationResult, modelRoot, modelFileName, encoding, writeOptionalPropertiesWithDefaultValue);
			if (serializationResult.Failed)
			{
				modelFileContent.Close();
				return;
			}
			
			var memoryStreamDictionary = new Dictionary<global::System.IO.MemoryStream, string>();
			foreach (var diagram in diagrams)
	        {
	            if (string.IsNullOrEmpty(diagram.Name))
	            {
	                throw new ArgumentException("Each diagram must have a name", "diagrams");
	            }
	            memoryStreamDictionary.Add(this.InternalSaveDiagram(serializationResult, diagram, diagramsFileName, encoding, writeOptionalPropertiesWithDefaultValue), diagram.Name);
	            if (serializationResult.Failed)
	            {
	                modelFileContent.Close();
	                memoryStreamDictionary.Keys.ToList<global::System.IO.MemoryStream>().ForEach(memoryStream => memoryStream.Close());
	                return;
	            }
	        }
			
			// Only write the contents if there's no error encountered during serialization.
			if (modelFileContent != null)
			{
				using (global::System.IO.FileStream fileStream = new global::System.IO.FileStream(modelFileName, global::System.IO.FileMode.Create, global::System.IO.FileAccess.Write, global::System.IO.FileShare.None))
				{
					using (global::System.IO.BinaryWriter writer = new global::System.IO.BinaryWriter(fileStream, encoding))
					{
						writer.Write(modelFileContent.ToArray());
					}
				}
			}
			
			using (var pkgOutputDoc = global::System.IO.Packaging.Package.Open(diagramsFileName, global::System.IO.FileMode.Create, global::System.IO.FileAccess.ReadWrite))
	        {
				foreach (var memoryStream in memoryStreamDictionary.Keys)
	            {
	                var bytes = memoryStream.ToArray();
	                var uri =  global::System.IO.Packaging.PackUriHelper.CreatePartUri(
	                        new Uri(string.Format("/diagrams/{0}", memoryStreamDictionary[memoryStream]), UriKind.Relative));
	                var part = pkgOutputDoc.CreatePart(uri, global::System.Net.Mime.MediaTypeNames.Text.Xml, global::System.IO.Packaging.CompressionOption.Maximum);
	                using (var partStream = part.GetStream(global::System.IO.FileMode.Create, global::System.IO.FileAccess.Write))
	                {
	                    partStream.Write(bytes, 0, bytes.Length);
	                }
	            }
			}
		}
		
		internal virtual void SaveDiagrams(DslModeling::SerializationResult serializationResult, DslDiagrams::Diagram[] diagrams, string diagramsFileName, global::System.Text.Encoding encoding, bool writeOptionalPropertiesWithDefaultValue)
		{
			#region Check Parameters
			if (serializationResult == null)
				throw new global::System.ArgumentNullException("serializationResult");
			if (diagrams == null)
				throw new global::System.ArgumentNullException("diagrams");
			if (string.IsNullOrEmpty(diagramsFileName))
				throw new global::System.ArgumentNullException("diagramsFileName");
			#endregion
			
			if (serializationResult.Failed)
				return;
				
			var memoryStreamDictionary = new Dictionary<global::System.IO.MemoryStream, string>();
			foreach (var diagram in diagrams)
	        {
				// HACK : Add validation rule on Diagram Name (!string.IsNullOrEmpty && Unique )
	            if (string.IsNullOrEmpty(diagram.Name))
	            {
	                throw new ArgumentException("Each diagram must have a name", "diagrams");
	            }
	            memoryStreamDictionary.Add(this.InternalSaveDiagram(serializationResult, diagram, diagramsFileName, encoding, writeOptionalPropertiesWithDefaultValue), diagram.Name);
	            if (serializationResult.Failed)
	            {
	                memoryStreamDictionary.Keys.ToList<global::System.IO.MemoryStream>().ForEach(memoryStream => memoryStream.Close());
	                return;
	            }
	        }
			
			using (var pkgOutputDoc = global::System.IO.Packaging.Package.Open(diagramsFileName, global::System.IO.FileMode.Create, global::System.IO.FileAccess.ReadWrite))
	        {
				foreach (var memoryStream in memoryStreamDictionary.Keys)
	            {
	                var bytes = memoryStream.ToArray();
	                var uri = new Uri(string.Format("/diagrams/{0}", memoryStreamDictionary[memoryStream]), UriKind.Relative);
	                var part = pkgOutputDoc.CreatePart(uri, global::System.Net.Mime.MediaTypeNames.Text.Xml, global::System.IO.Packaging.CompressionOption.Maximum);
	                using (var partStream = part.GetStream(global::System.IO.FileMode.Create, global::System.IO.FileAccess.Write))
	                {
	                    partStream.Write(bytes, 0, bytes.Length);
	                }
	            }
			}
		}
	}
	
}
