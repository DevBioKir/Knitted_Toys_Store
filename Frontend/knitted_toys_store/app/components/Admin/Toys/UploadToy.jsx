import React, { useState } from "react";
import axios from "axios";
import "./UploadPage.css"; // Путь к файлу стилей
import uploadToysFromExcel from "@/app/services/Admin/serviceUploadToysFromExcel";

export const ToyUploadPage = () => {
  const [zipFile, setZipFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  // Функция для загрузки ZIP-файла
  const handleFileUpload = async () => {
    if (!zipFile) {
      setErrorMessage("Пожалуйста, выберите ZIP файл для загрузки");
      return;
    }

    setLoading(true);
    setErrorMessage("");

    try {
      await uploadToysFromExcel(zipFile); // просто вызываем функцию
      alert("Игрушки успешно загружены!");
    } catch (error) {
      setErrorMessage("Произошла ошибка при загрузке. Попробуйте снова.");
    } finally {
      setLoading(false);
    }
  };

  // Функция для обработки выбора файла
  const handleFileChange = (e) => {
    setZipFile(e.target.files[0]);
  };

  return (
    <div className="container">

      <div className="upload-instructions">
          <h3>Инструкция по загрузке</h3>
          <p>1. Создать папку с расширением .zip.</p>
          <p>
            2. Скачайте{" "}
            <a
              href={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Templates/toy_download_template.xlsx`}
              target="_blank"
              rel="noopener noreferrer"
            >
              шаблон загрузки игрушек
            </a>
            .
          </p>
          <p>
            3. Заполните все поля в Excel-файле. Все изображения игрушек
            необходимо поместить в папку с расширением .zip.
          </p>
          <img
            src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Templates/example_excel_preview.jpg`}
            alt="Пример заполнения Excel"
            style={{
              width: "460px",
              border: "1px solid #ccc",
              marginTop: "1rem",
            }}
          />
          <p>
            4. Убедитесь, что в этой ZIP-папке находятся изображения, а в
            столбце Excel-файла «ImageFileName» указаны точные имена файлов с
            расширениями для каждого изображения игрушки.
          </p>
          <p>5. Загрузите ZIP файл с игрушками и изображениями.</p>
        </div>

        <div className="file-upload-section" style={{ marginTop: "0.3rem" }}>
          <h3>Загрузить ZIP файл</h3>
          <input type="file" accept=".zip" onChange={handleFileChange} />
          <button onClick={handleFileUpload} disabled={loading}>
            {loading ? "Загружаем..." : "Загрузить"}
          </button>

          {errorMessage && <p className="error-message">{errorMessage}</p>}
        </div>
        </div>
  );
};

export default ToyUploadPage;
