using DataBase.Abstractions;
using DataBase.Models;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProductsWork.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        private readonly IProductsDataAccess _entitiesDb;

        public string Reception
        {
            get => GetProperty(() => Reception);
            set => SetProperty(() => Reception, value.ToUpper());
        }


        public string Shipment
        {
            get => GetProperty(() => Shipment);
            set => SetProperty(() => Shipment, value.ToUpper());
        }

        public ObservableCollection<ProductCounter> UpdateReceptionGrid { get; set; }
        public ObservableCollection<ProductCounter> UpdateShipmentGrid { get; set; }

        public DelegateCommand ClearCommand { get; set; }

        public DelegateCommand GetReceptionInput { get; set; }

        public DelegateCommand GetShipmentInput { get; set; }

        public MainViewModel(IProductsDataAccess entitiesDataAccess)
        {
            _entitiesDb = entitiesDataAccess;

            _entitiesDb.AddGroupProducts(new List<Product>
            {
                new Product { Id = "304DB75F196000180001C13A", Name = "User1" },
                new Product { Id = "304DB75F196000180001C13B", Name = "User2" },
                new Product { Id = "304DB75F196000180001C13C", Name = "User3" },
                new Product { Id = "304DB75F196000180001C13D", Name = "User4" },
                new Product { Id = "304DB75F196000180001C13E", Name = "User5" },
                new Product { Id = "304DB75F196000180001C13F", Name = "User6" },
                new Product { Id = "304DB75F196000180001C131", Name = "User7" }
            });

            UpdateReceptionGrid = new ObservableCollection<ProductCounter>(entitiesDataAccess.GetAllReceptionProducts());

            UpdateShipmentGrid = new ObservableCollection<ProductCounter>(entitiesDataAccess.GetAllShipmentProducts());

            ClearCommand = new DelegateCommand(RemoveTables);

            GetReceptionInput = new DelegateCommand(AddReception);

            GetShipmentInput = new DelegateCommand(AddShipment);
        }

        private void AddReception()
        {            
            var parsedInput = ParseInput(Reception);

            foreach (var id in parsedInput)
            {                    
                try
                {
                    var productCounter = _entitiesDb.AddOrIncrementReceptionProductCounter(id);

                    if (productCounter != null)
                    {
                        var receptionProductCounter = UpdateReceptionGrid.FirstOrDefault(x => x.Id == productCounter.Id);

                        var shipmentProductCounter = UpdateShipmentGrid.FirstOrDefault(x => x.Id == productCounter.Id);

                        if (shipmentProductCounter != null)
                        {
                            var shipmentIndex = UpdateShipmentGrid.IndexOf(shipmentProductCounter);

                            UpdateShipmentGrid.RemoveAt(shipmentIndex);

                            if (shipmentProductCounter.Shipment > 1)
                            {
                                UpdateShipmentGrid.Insert(shipmentIndex, productCounter);
                            }
                        }

                        if (receptionProductCounter != null)
                        {
                            var receptionIndex = UpdateReceptionGrid.IndexOf(receptionProductCounter);

                            UpdateReceptionGrid.RemoveAt(receptionIndex);

                            UpdateReceptionGrid.Insert(receptionIndex, productCounter);
                        }

                        else
                        {
                            UpdateReceptionGrid.Add(productCounter);
                        }
                    }    
                }

                catch (Exception ex)
                {
                    //logging

                    throw;
                }
            }

            Reception = "";            
        }

        private void AddShipment()
        {
            var parsedInput = ParseInput(Shipment);

            foreach (var id in parsedInput)
            {
                try
                {
                    var productCounter = _entitiesDb.AddOrIncrementShipmentProductCounter(id);

                    if (productCounter != null)
                    {
                        var receptionProductCounter = UpdateReceptionGrid.FirstOrDefault(x => x.Id == productCounter.Id);

                        var shipmentProductCounter = UpdateShipmentGrid.FirstOrDefault(x => x.Id == productCounter.Id);

                        if (receptionProductCounter != null)
                        {
                            var receptionIndex = UpdateReceptionGrid.IndexOf(receptionProductCounter);

                            UpdateReceptionGrid.RemoveAt(receptionIndex);

                            if (receptionProductCounter.Reception > 1)
                            {
                                UpdateReceptionGrid.Insert(receptionIndex, productCounter);
                            }
                        }

                        if (shipmentProductCounter != null)
                        {
                            var shipmentIndex = UpdateShipmentGrid.IndexOf(shipmentProductCounter);

                            UpdateShipmentGrid.RemoveAt(shipmentIndex);

                            UpdateShipmentGrid.Insert(shipmentIndex, productCounter);
                        }

                        else
                        {
                            UpdateShipmentGrid.Add(productCounter);
                        }
                    }
                }

                catch (Exception ex)
                {
                    //logging

                    throw;
                }
            }

            Shipment = "";
        }

        private IEnumerable<string> ParseInput(string input)
        {
            var delimiterChars = new char[] { ' ', ',', '.', ':', '\t' };

            return input.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
        }

        private void RemoveTables()
        {
            _entitiesDb.RemoveAllProductsCounters();

            UpdateShipmentGrid.Clear();
            UpdateReceptionGrid.Clear();
        }
    }
}
