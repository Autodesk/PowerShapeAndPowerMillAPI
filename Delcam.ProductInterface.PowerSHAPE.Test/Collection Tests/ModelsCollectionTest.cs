// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Linq;
using System.Reflection;
using Autodesk.FileSystem;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for ModelsCollectionTest and is intended
    /// to contain all ModelsCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class ModelsCollectionTest : DatabaseEntitiesCollectionTest<PSModel>
    {
        #region Constructors

        public ModelsCollectionTest()
        {
            // Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        /// <summary>
        /// This test checks that the Model's Identifier is as expected
        /// </summary>
        [Test]
        public void IdentifierModelTest()
        {
            // Check that the collection Identifier matches the model
            var actualIdentifier =
                (string)
                _powerSHAPE.ActiveModel.GetType()
                           .GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance)
                           .GetValue(_powerSHAPE.ActiveModel, null);
            Assert.AreEqual("MODEL", actualIdentifier);
        }

        #endregion

        #region ModelsCollection Tests

        /// <summary>
        /// A test for getting a Model by name
        /// </summary>
        [Test]
        public void GetModelByNameTest()
        {
            //Open test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Check method works
            Assert.IsTrue(_powerSHAPE.Models["LevelTests"].Exists, "Failed to return correct model");
        }

        /// <summary>
        /// A test for getting a Model by number
        /// </summary>
        [Test]
        public void GetModelByNumberTest()
        {
            //Open test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Check method works
            Assert.AreEqual("LevelTests", _powerSHAPE.Models[1].Name, "Failed to return correct model");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearModelsTest()
        {
            // Open known model
            var newModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Remove all models
            _powerSHAPE.Models.Clear();

            // Check model has been removed
            Assert.AreEqual(0, _powerSHAPE.Models.Count, "Models were not removed from collection");
            Assert.IsFalse(newModel.Exists, "Models were not deleted from PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateEmptyModel
        /// </summary>
        [Test]
        public void CreateEmptyModelTest()
        {
            // Get number of open models
            var initialNumberOfModels = _powerSHAPE.Models.Count;

            // Open a new model
            _powerSHAPE.Models.CreateEmptyModel();

            // Check model has been opened
            Assert.AreNotEqual(initialNumberOfModels, _powerSHAPE.Models.Count, "Model not created");
        }

        /// <summary>
        /// A test for CreateModelFromFile
        /// </summary>
        [Test]
        public void CreateModelFromFileTest()
        {
            // Get number of open models
            var initialNumberOfModels = _powerSHAPE.Models.Count;

            // Open a model from a file
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Check model has been opened
            Assert.AreNotEqual(initialNumberOfModels, _powerSHAPE.Models.Count, "Model not opened");
        }

        /// <summary>
        /// A test for CreateModelFromFile when opening not readonly
        /// </summary>
        [Test]
        public void CreateModelFromFileEditableTest()
        {
            // Open a model from a file
            var activeModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Check model has been opened
            Assert.IsFalse(activeModel.IsReadOnly);
        }

        /// <summary>
        /// A test for CreateModelFromFile when opening readonly
        /// </summary>
        [Test]
        public void CreateModelFromFileReadonlyTest()
        {
            // Open a model from a file
            var activeModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL), true);

            // Check model has been opened
            Assert.IsTrue(activeModel.IsReadOnly);
        }

        /// <summary>
        /// Checks it does not crash when trying to open a model if no model is currently open
        /// </summary>
        [Test]
        public void CreateModelFromFileWhenNoModelsOpen()
        {
            _powerSHAPE.Models.Clear();

            Assert.IsFalse(_powerSHAPE.Models.Any());

            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            Assert.IsTrue(_powerSHAPE.Models.Any());
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveModelByNameTest()
        {
            // Get number of models currently open
            var initialNumberOfModels = _powerSHAPE.Models.Count;

            // Open known model
            var newModel = _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Remove model
            _powerSHAPE.Models.Remove("LevelTests");

            // Check model has been removed
            Assert.AreEqual(initialNumberOfModels, _powerSHAPE.Models.Count, "Model was not removed from collection");
            Assert.IsFalse(newModel.Exists, "Model was not deleted from PowerSHAPE");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveModelTest()
        {
            // Get number of models currently open
            var initialNumberOfModels = _powerSHAPE.Models.Count;

            // Open known model
            var newModel =
                _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Remove model
            _powerSHAPE.Models.Remove(newModel);

            // Check model has been removed
            Assert.AreEqual(initialNumberOfModels, _powerSHAPE.Models.Count, "Model was not removed from collection");
            Assert.IsFalse(newModel.Exists, "Model was not deleted from PowerSHAPE");
        }

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveModelAtTest()
        {
            // Get number of models currently open
            var initialNumberOfModels = _powerSHAPE.Models.Count;

            // Open known model
            var newModel =
                _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Remove model
            _powerSHAPE.Models.RemoveAt(1);

            // Check model has been removed
            Assert.AreEqual(initialNumberOfModels, _powerSHAPE.Models.Count, "Model was not removed from collection");
            Assert.IsFalse(newModel.Exists, "Model was not deleted from PowerSHAPE");
        }

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void ContainsModelTest()
        {
            // Create Delcam file
            var modelFile = new File(TestFiles.LEVELS_MODEL);

            // Open known model
            _powerSHAPE.Models.CreateModelFromFile(modelFile);

            // Check Contains returns true
            Assert.IsTrue(_powerSHAPE.Models.Contains(modelFile),
                          "Collection does not return that it contains the model");
        }

        #endregion
    }
}