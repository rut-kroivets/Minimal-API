import axios from 'axios';

const apiUrl = "http://localhost:5297"
// Config Defaults - הגדרת כתובת API כ-default
axios.defaults.baseURL = apiUrl;

// Interceptor לטיפול בשגיאות ב-response
axios.interceptors.response.use(
  (response) => response, // אם התקבל response בהצלחה, החזר את ה-response כמו שהוא
  (error) => {
    // אם קיבלנו שגיאה ב-response, עבור על השגיאה ורושם ללוג
    console.error('Error in API response:', error.response);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`${apiUrl}/items`);
    return result.data;
},

  addTask: async(name)=>{
    console.log('addTask', name)
    try {
      const result = await axios.post(`${apiUrl}/items`, { name }); // POST request ליצירת משימה חדשה
      console.log('addTask', result.data);
      return result.data;
    } catch (error) {
      console.error('Error adding task:', error);
      throw error;
    }
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    try {
      const result = await axios.put(`${apiUrl}/items/${id}`, { isComplete }); // PUT request לעדכון סטטוס השלמה של משימה
      console.log('setCompleted', result.data);
      return result.data;
    } catch (error) {
      console.error('Error setting task completion:', error);
      throw error;
    }
    
  },

  deleteTask:async(id)=>{
    console.log('deleteTask')
    try {
      const result = await axios.delete(`${apiUrl}/items/${id}`); // DELETE request למחיקת משימה
      console.log('deleteTask', result.data);
      return result.data;
    } catch (error) {
      console.error('Error deleting task:', error);
      throw error;
    }
  }
};