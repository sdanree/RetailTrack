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
-- Table structure for table `providers`
--

DROP TABLE IF EXISTS `providers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `providers` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `BusinessName` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RUT` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `providers`
--

LOCK TABLES `providers` WRITE;
/*!40000 ALTER TABLE `providers` DISABLE KEYS */;
INSERT INTO `providers` VALUES ('045906f5-d783-45e9-bd41-a52ae4553011','luma srl','otro texto','7986969860','vickyhouse','3456789','su local'),('2996c99f-e748-431c-9a32-6c740b9da135','23223','','23323','2332','23323','23232323'),('29997ec0-f566-4641-83d4-45a73992cd17','aranjuez SA','una tienda sin igual','213165465498','cuborubik','51651321654654','su local preferido'),('320c85b0-2534-4b0d-a224-fcf65bfe4fd6','mateina srl','una casa a la que le gusta el mate','7897987','mateina','89778979879','mate esquina termo entre matera y bombilla'),('692e94ff-83f9-4622-ad85-f5ad1a6b9609','12321','','123','213','123','123'),('70708122-e6bc-42cc-8149-828abeb7f8cc','calafate SA','','4239472309','calafete pilchitas','129307419273','algun lugar de la patagonia argentina'),('72f98318-97f2-4fb4-bb96-0f3c92dbd070','malu','sdafasdfasd','00707','muluhouse','90709709','sdfjasdfu'),('733daca6-1164-4e1a-9c82-c5dbd6a97637','nombrecomercial','descripcion','234423','nombre fantacia','sfdfsdf','sefsdds'),('b3af094d-0e07-44b7-aec1-729c432c7ade','Cuadrenos Srl','','7897','Cuaderneria','89','fasd'),('b8f88fc2-ac05-4ff5-95ee-378a5cb3ee98','otro provider','sadfasdfasdfsdfsdfsdafasdfsdafsadfsdafdsa','0909909','444444444444','9233423423','sfasdfasfsdfasdfsd'),('e089e622-a83e-4697-8ec9-cadf110c5795','121212121221','112212121','1212','121212122121','21211221','122121212'),('e12a2482-cbfa-486a-90e6-976ae781f1d8','aranjuez SA','una tienda sin igual','213165465498','cuborubik','51651321654654','su local preferido'),('faecdce8-d264-42e5-8f1e-9698bca94f34','petpito','1321321','13213232','ferias','32132213','3213132');
/*!40000 ALTER TABLE `providers` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-24 22:06:52
