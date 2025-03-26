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
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DesignId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ProductStatusId` int NOT NULL,
  `GeneralPrice` decimal(65,30) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Products_ProductStatusId` (`ProductStatusId`),
  KEY `IX_Products_DesignId` (`DesignId`),
  CONSTRAINT `FK_Products_Designs_DesignId` FOREIGN KEY (`DesignId`) REFERENCES `designs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Products_ProductStatuses_ProductStatusId` FOREIGN KEY (`ProductStatusId`) REFERENCES `productstatuses` (`Status_Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES ('aa44a508-0162-4698-8e63-182efc4b7d97','afsd','asdfasdf','08dd1fae-3b82-4728-8c1e-63365a8f559d',1,3333.000000000000000000000000000000),('cc78a325-5a27-4d3b-bee8-99a018224bc7','cucu','casa','08dd29ce-7514-4ad9-8f06-408543a06e15',1,55555.000000000000000000000000000000),('d32a19d9-9875-4b9a-95d2-3015710fc0fc','mono loco','mono loco','08dd52ad-f87c-4ab0-82f6-f61e7a03455a',2,700.000000000000000000000000000000),('d860ff27-5c33-459d-85f3-6f40e6f1ccac','lauti','enano liceal','08dd52ad-f87c-4ab0-82f6-f61e7a03455a',3,66.000000000000000000000000000000),('f837f6af-4063-458a-8f48-200d0ce8545e','xcvdkl;j','2030231','08dd29ce-7514-4ad9-8f06-408543a06e15',1,56151.000000000000000000000000000000);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
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
