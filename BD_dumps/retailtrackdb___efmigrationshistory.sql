-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: localhost    Database: retailtrackdb
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20241203191448_InitialCreate','8.0.2'),('20241204172737_AddDesignIdToProducts','8.0.2'),('20241209191629_AddMaterialAndMaterialTypeModels','8.0.2'),('20241209192747_AddComisionToDesing','8.0.2'),('20241209211138_AddPriceToDesing','8.0.2'),('20241210014205_UpdateProductAndMovement','8.0.2'),('20241211184816_AddMaterialIdToProduct','8.0.2'),('20241212202306_MakeDescriptionNullable','8.0.2'),('20241212224917_AddPaymentMethodandGoodReceipt','8.0.2'),('20241213224740_UpdateGoodsReceiptSchema','8.0.2'),('20241216225134_RestructureReceiptAndPayment','8.0.2'),('20241218170756_AddMaterialSizeRelation','8.0.2'),('20241219163212_AmountInReceiptPayments','8.0.2'),('20241219212515_AjustsInReceipt','8.0.2'),('20250219163956_AddReceiptExternalCode','8.0.2'),('20250221210502_AddPurchaseOrders','8.0.2'),('20250226170511_AddLastProviderToMaterialSize','8.0.2'),('20250304002153_attpurchaseOrderNumber','8.0.2'),('20250312172324_AddOrdersAndStockRelationships','8.0.2'),('20250313225305_FixProductForeignKeys','8.0.2'),('20250317202644_UpdateProductVariants','8.0.2'),('20250320162003_AddAvailableToProductStock','8.0.2'),('20250320165050_UpdateProductStatusEnum','8.0.2');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-24 22:06:51
