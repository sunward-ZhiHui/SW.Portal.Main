using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class FbOutputCartonsQueryRepository : DbConnector, IFbOutputCartonsQueryRepository
    {
        public FbOutputCartonsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public  async Task<long> Delete(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("FbOutputCartonID", id);

                        var query = "DELETE  FROM FbOutputCartons WHERE FbOutputCartonID = @FbOutputCartonID";


                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<FbOutputCartons>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM FbOutputCartons";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FbOutputCartons>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<FbOutputCartons>> GetAllCartonsCountAsync(string PalletNo)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("PalletNo", PalletNo);
                var query = @"SELECT 
                     SUM(CASE WHEN FullCartonQty > 0 THEN FullCartonQty ELSE 0 END) AS TotalFullCartonQty,
                     SUM(CASE WHEN LooseCartonQty > 0 THEN LooseCartonQty ELSE 0 END) AS TotalLooseCartonQty,
                     COUNT(CASE WHEN FullCartonQty > 0 THEN 1 END) AS CountFullCartonQty,
                     COUNT(CASE WHEN LooseCartonQty > 0 THEN 1 END) AS CountLooseCartonQty FROM   FbOutputCartons WHERE  PalletNo = @PalletNo";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FbOutputCartons>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<FbOutputCartons>> GetAllFullCartonsAsync(string PalletNo)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("PalletNo", PalletNo);
                var query = @"select * From FbOutputCartons Where PalletNo = @PalletNo and  FullCartonQty >0";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FbOutputCartons>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<FbOutputCartons>> GetAllLooseCartonsAsync(string PalletNo)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("PalletNo", PalletNo);
                var query = @"select * From FbOutputCartons Where PalletNo = @PalletNo and  LooseCartonQty > 0";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FbOutputCartons>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<long> Insert(FbOutputCartons fbOutputCartons)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("BatchNo", fbOutputCartons.BatchNo);
                        parameters.Add("CartonNo", fbOutputCartons.CartonNo);
                        parameters.Add("FullCarton", fbOutputCartons.FullCarton);
                        parameters.Add("FullCartonQty", fbOutputCartons.FullCartonQty);
                        parameters.Add("Description", fbOutputCartons.Description);
                        parameters.Add("ProductionOrderNo", fbOutputCartons.ProductionOrderNo);
                        parameters.Add("PalletNo", fbOutputCartons.PalletNo);
                        parameters.Add("LocationName", fbOutputCartons.LocationName);
                        parameters.Add("ItemNo", fbOutputCartons.ItemNo);
                        parameters.Add("LooseCartonQty", fbOutputCartons.LooseCartonQty);
                        parameters.Add("SessionId", fbOutputCartons.SessionId);
                        parameters.Add("AddedByUserID", fbOutputCartons.AddedByUserID);
                        parameters.Add("AddedDate", fbOutputCartons.AddedDate);
                        parameters.Add("StatusCodeID", fbOutputCartons.StatusCodeID);

                        var query = @"INSERT INTO FbOutputCartons(BatchNo,CartonNo,FullCarton,FullCartonQty,Description,ProductionOrderNo,PalletNo,LocationName,ItemNo,LooseCartonQty,SessionId,AddedByUserID,AddedDate,StatusCodeID)
                         VALUES (@BatchNo,@CartonNo,@FullCarton,@FullCartonQty,@Description,@ProductionOrderNo,@PalletNo,@LocationName,@ItemNo,@LooseCartonQty,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID)";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                    }


                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }


            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

        public  async Task<long> Update(FbOutputCartons fbOutputCartons)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("FbOutputCartonID", fbOutputCartons.FbOutputCartonID);
                        parameters.Add("BatchNo", fbOutputCartons.BatchNo);
                        parameters.Add("CartonNo", fbOutputCartons.CartonNo);
                        parameters.Add("FullCarton", fbOutputCartons.FullCarton);
                        parameters.Add("FullCartonQty", fbOutputCartons.FullCartonQty);
                        parameters.Add("Description", fbOutputCartons.Description);
                        parameters.Add("ProductionOrderNo", fbOutputCartons.ProductionOrderNo);
                        parameters.Add("PalletNo", fbOutputCartons.PalletNo);
                        parameters.Add("LocationName", fbOutputCartons.LocationName);
                        parameters.Add("ItemNo", fbOutputCartons.ItemNo);
                        parameters.Add("LooseCartonQty", fbOutputCartons.LooseCartonQty);
                        
                        parameters.Add("ModifiedByUserID", fbOutputCartons.ModifiedByUserID);
                        parameters.Add("ModifiedDate", fbOutputCartons.ModifiedDate);
                       

                        var query = @" UPDATE FbOutputCartons SET BatchNo = @BatchNo,CartonNo = @CartonNo,FullCarton=@FullCarton,FullCartonQty=@FullCartonQty,Description=@Description,
                           ProductionOrderNo = @ProductionOrderNo,PalletNo = @PalletNo,LocationName =@LocationName, ItemNo =@ItemNo ,LooseCartonQty =@LooseCartonQty,ModifiedByUserID =@ModifiedByUserID,ModifiedDate =@ModifiedDate WHERE FbOutputCartonID = @FbOutputCartonID";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> DispensedMeterialInsert(DispensedMeterial dispensedmeterial)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("MaterialName", dispensedmeterial.MaterialName);
                        parameters.Add("QCReference", dispensedmeterial.QCReference);                     
                        parameters.Add("SubLotNo", dispensedmeterial.SubLotNo);
                        parameters.Add("Description", dispensedmeterial.Description);
                        parameters.Add("ProductionOrderNo", dispensedmeterial.ProductionOrderNo);
                        parameters.Add("BatchNo", dispensedmeterial.BatchNo);
                        parameters.Add("TareWeight", dispensedmeterial.TareWeight);
                        parameters.Add("ActualWeight", dispensedmeterial.ActualWeight);
                        parameters.Add("UOM", dispensedmeterial.UOM);
                        parameters.Add("PrintLabel", dispensedmeterial.PrintLabel);
                        parameters.Add("SessionId", dispensedmeterial.SessionId);
                        parameters.Add("AddedByUserID", dispensedmeterial.AddedByUserID);
                        parameters.Add("AddedDate", dispensedmeterial.AddedDate);
                        parameters.Add("StatusCodeID", dispensedmeterial.StatusCodeID);

                        var query = @"INSERT INTO DispensedMeterial(MaterialName,QCReference,SubLotNo,Description,ProductionOrderNo,BatchNo,TareWeight,ActualWeight,UOM,PrintLabel,SessionId,AddedByUserID,AddedDate,StatusCodeID)
                         VALUES (@MaterialName,@QCReference,@SubLotNo,@Description,@ProductionOrderNo,@BatchNo,@TareWeight,@ActualWeight,@UOM,@PrintLabel,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID)";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                    }


                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }


            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

    }





}
