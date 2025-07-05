import React, { useState } from "react";
import { Button } from "antd";
import axios from "axios";
import "./UploadPage.css"; // Путь к файлу стилей
import exportToysToExcel from "@/app/services/Admin/ServiceUpdatePriceToys/serviceExportToysToExcel";
import importPricesFromExcel from "@/app/services/Admin/ServiceUpdatePriceToys/serviceImportPricesFromExcel";

export const ToyUpdatePricePage = () => {
  const [excelFile, setExcelFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [exportLoading, setExportLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  // Функция для экспорта данных в Excel-файла
  const handleFileExport = async (e) => {
    e.preventDefault();
    e.stopPropagation();
    //e.stopImmediatePropagation();
    if (e.stopImmediatePropagation) {
      e.stopImmediatePropagation();
    }

    setExportLoading(true);
    setErrorMessage("");

    try {
      await exportToysToExcel();
      console.log("Данные успешно экспортированы в Excel!");
      //alert("Данные успешно экспортированы в Excel!");
    } catch (error) {
      console.error("Ошибка экспорта:", error);
      setErrorMessage("Произошла ошибка при экспорте. Попробуйте снова.");
    } finally {
      setExportLoading(false);
    }
  };

  // Функция для импорта изменений из Excel-файла
  const handleFileImport = async () => {
    if (!excelFile) {
      setErrorMessage("Пожалуйста, выберите Excel файл для загрузки");
      return;
    }

    setLoading(true);
    setErrorMessage("");

    try {
      await importPricesFromExcel(excelFile);
      alert("Изменения успешно импортированы!");
      setExcelFile(null); // Сбрасываем выбранный файл
    } catch (error) {
      console.error("Ошибка импорта:", error);
      setErrorMessage("Произошла ошибка при импорте. Попробуйте снова.");
    } finally {
      setLoading(false);
    }
  };

  // Функция для обработки выбора файла
  const handleFileChange = (e) => {
    setExcelFile(e.target.files?.[0] || null);
    setErrorMessage(""); // Сбрасываем ошибку при выборе нового файла
  };

  return (
    <div className="container">
      <div className="upload-instructions">
        <h3>Инструкция по изменению цен на игрушки</h3>
        <p>1. Экспортируйте файл с игрушками</p>
        <button
          type="button"
          className="upload-button"
          onClick={handleFileExport}
          disabled={exportLoading}
        >
          {exportLoading ? "Экспортируем..." : "Экспорт"}
        </button>
        <p>2. Измените цены в Excel-файле.</p>
        <p>3. Загрузите измененный файл обратно на сайт.</p>
        <div className="file-upload-section" style={{ marginTop: "0.3rem" }}>
          <input type="file" accept=".xlsx" onChange={handleFileChange} />

          <button
            type="button"
            className="upload-button"
            onClick={handleFileImport}
            disabled={loading || !excelFile}
          >
            {loading ? "Загружаем..." : "Загрузить"}
          </button>

          {errorMessage && <p className="error-message">{errorMessage}</p>}
        </div>
        <p>
          4. Проверьте каталог игрушек, если цена не изменилась, обновите
          страницу каталога или попробуйте снова загрузить файл.
        </p>
      </div>
    </div>
  );
};

export default ToyUpdatePricePage;
