# -*- coding: utf-8 -*-

from __future__ import unicode_literals
import socketio
import os


from django.shortcuts import redirect  # render ,
from django.template.response import TemplateResponse as render
from django.http import HttpResponse
from src.local_cloud.Query_Search import select_data, create_DB, actuate_DB
from src.local_cloud.qt_rcv import  rcv, actuateData
import json
from apps.home.views import sio


async_mode = None

basedir = os.path.dirname(os.path.realpath(__file__))
# sio = socketio.Server(async_mode='eventlet',cors_allowed_origins=['http://127.0.0.1:8000','http://localhost:8000','http://0.0.0.0:8000'])\

#web
@sio.on('connect', namespace='/web')
def connect_web(sid, data):
    print('[INFO] Web client connected: {}'.format(sid))



@sio.on('disconnect', namespace='/web')
def disconnect_web(sid):
    print('[INFO] Web client disconnected: {}'.format(sid))


@sio.on('new_data', namespace='/web')
def newData(sid,data):
    sio.emit('actuateData',data,namespace='/movement')

# arm control 

    ## Pivot Control 

@sio.on('bottom_pivot', namespace='/web')
def bottomPivotData(sid,data):
    sio.emit('bottomPivot',data,namespace='/arm')

@sio.on('middle_pivot', namespace='/web')
def middlePivotData(sid,data):
    sio.emit('middlePivot',data,namespace='/arm')

@sio.on('upper_pivot', namespace='/web')
def upperPivotData(sid,data):
    sio.emit('upperPivot',data,namespace='/arm')
    ## Deck Control 

@sio.on('bottom_deck', namespace='/web')
def bottomDeckData(sid,data):
    sio.emit('bottomDeck',data,namespace='/arm')


    ## Fork Control 

@sio.on('fork_control', namespace='/web')
def forkData(sid,data):
    sio.emit('forkControl',data,namespace='/arm')


#cv
@sio.on('connect', namespace='/cv')
def connect_cv(sid, data):
    print('[INFO] CV client connected: {}'.format(sid))



@sio.on('disconnect', namespace='/cv')
def disconnect_cv(sid):
    print('[INFO] CV client disconnected: {}'.format(sid))


@sio.on('cv2server',namespace='/cv')
def handle_cv_message_digital(sid,message):
    sio.emit('server2webvirtual', message, namespace='/web')

@sio.on('cv2serverphysical',namespace='/cv')
def handle_cv_message_physical(sid,message):
    sio.emit('server2webphysical', message, namespace='/web')

#dashboard
@sio.on('connect', namespace='/dashboard')
def connect_dashboard(sid, data):
    print('[INFO] dashboard connected: {}'.format(sid))
    

@sio.on('disconnect', namespace='/dashboard')
def disconnect_dashboard(sid):
    print('[INFO] dashboard disconnected: {}'.format(sid))


@sio.on('sensedData',namespace='/dashboard')
def handle_sensed_data(sid,message):
    sio.emit('sensedData', message, namespace='/web')

@sio.on('gazebo_event',namespace='/gazebo')
def handle_sensed_data(sid,message):
    sio.emit('gazeboData', message, namespace='/manga')

#this is to send to the socketio the jointstates message for the arm 
@sio.on('arm_event',namespace='/arm_namespace')
def handle_sensed_data(sid,message):
    sio.emit('armData', message, namespace='/arm_control')


@sio.on('connect', namespace='/dynamicDB')
def connect_QT(sid,data):
    print("Hi from".format(sid))
    print('[INFO] Create_DynamicDB connected: {}'.format(sid))




def main(request):
    return render(request, "home/dashboard_layout.html")
    


def actuate_data(request):

    if request.is_ajax():
        speed = request.POST.get('speed_control')
        print("_____________________",speed)
        # actuate_DB(speed, 'control_speed')
    return render(request, "actuate/actuate_data.html", {'actuateData': json.dumps(actuateData())})
#
def steering_angle(request):
    if request.is_ajax():
        angle = request.POST.get('steering_angle')
        print("_____________________",angle)
        # actuate_DB(angle, 'steering_angle')
    return render(request, "actuate/actuate_data.html")

def direction(request):
    if request.is_ajax():
        direction = request.POST.get('direction')
        print("_____________________",direction)
        # actuate_DB(direction, 'direction')
    return render(request, "actuate/actuate_data.html")



