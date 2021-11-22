-- ----------------------------
-- Table structure for departamentos
-- ----------------------------
CREATE TABLE IF NOT EXISTS departamentos (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server  INTEGER,
nombre  TEXT
);

-- ----------------------------
-- Table structure for ciudades
-- ----------------------------
CREATE TABLE IF NOT EXISTS ciudades (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server  INTEGER,
id_server_departamento  INTEGER,
nombre  TEXT
);

-- ----------------------------
-- Table structure for sectores
-- ----------------------------
CREATE TABLE IF NOT EXISTS sectores (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server  INTEGER,
nombre  TEXT
);

-- ----------------------------
-- Table structure for fincas
-- ----------------------------
CREATE TABLE IF NOT EXISTS fincas (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server  INTEGER,
id_server_ciudad  INTEGER,
nombre  TEXT,
id_server_sector  INTEGER,
activo  INTEGER
);

-- ----------------------------
-- Table structure for usuarios
-- ----------------------------
CREATE TABLE IF NOT EXISTS usuarios (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server  INTEGER,
usuario  TEXT,
pass  TEXT,
nombres  TEXT,
apellido  TEXT,
activo  INTEGER
);

-- ----------------------------
-- Table structure for visitas
-- ----------------------------
CREATE TABLE IF NOT EXISTS visitas (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server_finca  INTEGER,
id_server_usuario  INTEGER,
id_server_tipo_visita  INTEGER,
temperatura_minima  REAL,
temperatura  REAL,
temperatura_maxima  REAL,
humedad_relativa  REAL,
fecha  TEXT,
estado_sync  INTEGER,
fecha_sync  TEXT
);

-- ----------------------------
-- Table structure for recorrido_visita
-- ----------------------------
CREATE TABLE IF NOT EXISTS recorrido_visita (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_visita  INTEGER,
latitud  REAL,
longitud  REAL,
fecha  TEXT,
FOREIGN KEY (id_visita) REFERENCES visitas (id)
);

-- ----------------------------
-- Table structure for punto_captura
-- ----------------------------
CREATE TABLE IF NOT EXISTS punto_captura (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_visita  INTEGER,
id_server_tipo  INTEGER,
fecha  TEXT,
FOREIGN KEY (id_visita) REFERENCES visitas (id)
);

-- ----------------------------
-- Table structure for planta_punto_captura
-- ----------------------------
CREATE TABLE IF NOT EXISTS planta_punto_captura (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_punto_captura  INTEGER,
id_server_edad  INTEGER,
latitud  REAL,
longitud  REAL,
fecha  TEXT,
FOREIGN KEY (id_punto_captura) REFERENCES punto_captura (id)
);

-- ----------------------------
-- Table structure for indicadores_punto_captura
-- ----------------------------
CREATE TABLE IF NOT EXISTS indicadores_punto_captura (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_planta_punto_captura  INTEGER,
id_server_indicador  INTEGER,
valor  REAL,
valor_texto  TEXT,
FOREIGN KEY (id_planta_punto_captura) REFERENCES planta_punto_captura (id)
);

-- ----------------------------
-- Table structure for fotos_indicador
-- ----------------------------
CREATE TABLE IF NOT EXISTS fotos_indicador (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_lectura_indicador  INTEGER,
url  TEXT,
estado_sync  INTEGER,
fecha_sync  TEXT,
FOREIGN KEY (id_lectura_indicador) REFERENCES indicadores_punto_captura (id)
);

-- ----------------------------
-- Table structure for precipitaciones
-- ----------------------------
CREATE TABLE IF NOT EXISTS precipitaciones (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server_finca  INTEGER,
id_server_usuario  INTEGER,
fecha  TEXT,
valor  REAL,
estado_sync  INTEGER,
fecha_sync  TEXT
);

-- ----------------------------
-- Table structure for sincronizaciones
-- ----------------------------
CREATE TABLE IF NOT EXISTS sincronizaciones (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server_usuario  INTEGER,
fecha  TEXT
);

-- ----------------------------
-- Table structure for errores
-- ----------------------------
CREATE TABLE IF NOT EXISTS errores (
id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
id_server_usuario INTEGER,
id_server_servicio  INTEGER,
fecha TEXT,
mensaje TEXT,
estado_sync INTEGER,
fecha_sync TEXT
);